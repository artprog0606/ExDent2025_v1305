import { IController, IFormController, IHttpService, IPromise, IQService, ITimeoutService } from 'angular';
import type { CustomerContactsFields, Form, OnAddEditAddressCallbackData, ContactId, AddressListConfig } from '../types';
import type { ICustomerContact } from '../../../myaccount/types';
import type { ICountry, IIpZone } from '../../zone/types';
import { type Response, isResponseError } from '../../../@types/http';

export class AddEditAddressFormController implements IController {
    private isShowName: boolean = false;
    private customerId: number | null = null;
    // timerChange,
    private processContactTimer: IPromise<any> | null = null;
    fields: CustomerContactsFields | undefined = undefined;
    form: Form = {};
    addressListMaxHeight: number = 0;
    citiesListMaxHeight: number = 0;
    requiredValidationEnabled: boolean = false;
    autocompleteAlt: boolean = false;
    onAddEditAddress: ((data: OnAddEditAddressCallbackData) => void) | undefined = undefined;
    addEditAddressHtmlForm: IFormController | null = null;
    formData: Form | undefined = undefined;
    addressSelected: ICustomerContact | undefined = undefined;
    zoneUpdateOnAdd: boolean | undefined = undefined;
    contactId: ContactId = undefined;

    /* @ngInject */
    constructor(
        private readonly addressService: any,
        private readonly addressListConfig: AddressListConfig,
        private readonly $q: IQService,
        private readonly $http: IHttpService,
        private readonly urlHelper: any,
        private readonly $timeout: ITimeoutService,
        private readonly modalService: any,
        private readonly zoneService: any,
    ) {}

    $onInit() {
        this.requiredValidationEnabled = this.addressListConfig.requiredValidationEnabled;
        this.autocompleteAlt = this.autocompleteAlt != null ? this.autocompleteAlt : this.addressListConfig.autocompleteAlt;
        this.clearFormData();
        if (this.formData) {
            this.form = this.formData;
        }
        return this.addressService
            .getFields(this.isShowName, this.customerId)
            .then((response: CustomerContactsFields) => {
                return (this.fields = this.fields || this.checkFieldsVisible(response));
            })
            .then((fields: CustomerContactsFields) => {
                if (!fields.IsShowFullAddress) {
                    this.clearFullAddressData();
                }
                this.initListsMaxHeight();
                if (fields.IsShowCountry) {
                    return this.getCountries();
                }
            })
            .then((countries: ICountry[] | undefined) => {
                if (countries != null) {
                    return this.getSelectedCountry(countries);
                }
            })
            .catch((error: unknown) => {
                console.error(error);
            });
    }

    $postLink() {
        if (this.fields?.IsShowFullAddress) {
            this.clearFullAddressData();
        }
    }

    checkFieldsVisible(fields: CustomerContactsFields) {
        if (this.addressListConfig.overrideFields.visible) {
            if (Array.isArray(this.addressListConfig.overrideFields.visible)) {
                for (const value of this.addressListConfig.overrideFields.visible) {
                    if (fields[value] != null) {
                        this.overrideFieldsForm(fields, value, true);
                    }
                }
            } else {
                throw new Error('AddressList: field "visible" should be like array of strings');
            }
        }
        return fields;
    }

    overrideFieldsForm<K extends keyof CustomerContactsFields, V extends CustomerContactsFields[K]>(
        fields: CustomerContactsFields,
        key: K,
        value: V,
    ) {
        fields[key] = value;
    }

    clearFullAddressData = () => {
        this.form.house = null;
        this.form.apartment = null;
        this.form.structure = null;
        this.form.entrance = null;
        this.form.floor = null;
    };

    initListsMaxHeight = () => {
        if (this.fields) {
            this.addressListMaxHeight =
                50 *
                    ((this.fields.IsShowCity ? 1 : 0) +
                        (this.fields.IsShowDistrict ? 1 : 0) +
                        (this.fields.IsShowState ? 1 : 0) +
                        (this.fields.IsShowCountry ? 1 : 0) +
                        (this.fields.IsShowZip ? 1 : 0)) || 50;
            if (!this.fields?.IsShowFullAddress) {
                this.citiesListMaxHeight =
                    50 *
                        ((this.fields.IsShowState ? 1 : 0) +
                            (this.fields.IsShowCountry ? 1 : 0) +
                            (this.fields.IsShowZip ? 1 : 0) +
                            2 * (this.fields.IsShowAddress ? 1 : 0)) || 50;
            }
        }
    };

    getCountries = async (): Promise<ICountry[] | undefined> => {
        if (this.form.countries != null) {
            return this.form.countries;
        } else {
            try {
                const { data } = await this.$http.get<ICountry[]>(this.urlHelper.getAbsUrl('location/GetCountries', true));
                this.form.countries = data;
                return data;
            } catch (e) {
                console.error(e);
            }
        }
    };

    getSelectedCountry = (countries: ICountry[]) => {
        return this.$q
            .when(this.form.countryId != null && this.form.countryId !== 0 ? { CountryId: this.form.countryId } : this.zoneService.getCurrentZone())
            .then((zone) => {
                let country;

                for (let i = countries.length - 1; i >= 0; i--) {
                    if (countries[i].CountryId === zone.CountryId) {
                        country = countries[i];
                        break;
                    }
                }

                return (this.form.country = country);
            });
    };

    processCity = (zone: IIpZone, timeout: number) => {
        if (this.processContactTimer != null) {
            this.$timeout.cancel(this.processContactTimer);
        }

        return (this.processContactTimer = this.$timeout(
            () => {
                if (zone != null) {
                    this.form.region = zone.Region;
                    this.form.district = zone.District;
                    this.form.countryId = zone.CountryId;
                    this.form.zip = zone.Zip;
                }
                this.form.byCity = zone == null;

                this.addressService.processAddress(this.form, this.customerId).then((data: Response<IIpZone>) => {
                    if (!isResponseError(data) && data.result) {
                        this.form.countryId = data.obj?.CountryId;
                        this.form.region = data.obj?.Region;
                        this.form.district = data.obj?.District;
                        this.form.zip = data.obj?.Zip;
                        if (this.form.countries) {
                            this.getSelectedCountry(this.form.countries);
                        }
                    }
                });
            },
            timeout != null ? timeout : 700,
        ));
    };

    processAddress = (data: IIpZone, timeout?: number) => {
        if (this.fields != null && !this.fields.UseAddressSuggestions) {
            return;
        }

        if (this.processContactTimer != null) {
            this.$timeout.cancel(this.processContactTimer);
        }
        return (this.processContactTimer = this.$timeout(
            () => {
                this.form.byCity = false;
                if (data != null && data.Zip) {
                    this.form.zip = data.Zip;
                } else {
                    this.addressService.processAddress(this.form, this.customerId).then((data: Response<IIpZone>) => {
                        if (!isResponseError(data) && data.result) {
                            this.form.zip = data.obj?.Zip;
                        }
                    });
                }
            },
            timeout != null ? timeout : 700,
        ));
    };

    processCountry = () => {
        this.form.countryId = this.form.country?.CountryId;
    };

    cancelAdding = (modalId: string) => {
        this.modalService.close(modalId);
        if (this.fields?.IsShowFullAddress) {
            this.clearFullAddressData();
        }
        this.clearFormData();
    };

    save = () => {
        const obj = this.getObjectForUpdate(this.form);
        this.addressService.addUpdateCustomerContact(obj, this.customerId).then((response: ICustomerContact) => {
            if (response !== null) {
                if (this.zoneUpdateOnAdd === true) {
                    this.zoneService.setCurrentZone(
                        response.City,
                        response.CityId,
                        response.CountryId,
                        response.Region,
                        response.Country,
                        response.Zip,
                        response.District,
                    );
                }

                this.addressService.getAddresses(this.customerId).then((response: ICustomerContact[]) => {
                    const addressSelected = this.addressService.findItemForSelect(response, this.contactId);
                    if (this.onAddEditAddress) {
                        this.onAddEditAddress({ formData: this.form, contacts: response, addressSelected });
                    }
                    this.close();
                });
            }
        });
    };

    close = () => {
        this.modalService.close('modalAddress');
        if (this.fields?.IsShowFullAddress) {
            this.clearFullAddressData();
        }
        this.clearFormData();
    };

    clearFormData = () => {
        this.form.contactId = undefined;
        this.form.fio = undefined;
        this.form.firstName = undefined;
        this.form.lastName = undefined;
        this.form.patronymic = undefined;
        this.form.countryId = undefined;
        this.form.country = undefined;
        this.form.region = undefined;
        this.form.city = undefined;
        this.form.district = undefined;
        this.form.street = undefined;
        this.form.zip = undefined;
        if (this.addEditAddressHtmlForm != null) {
            this.addEditAddressHtmlForm.$setPristine();
        }
    };

    getObjectForUpdate = (form: Form) => {
        const account: ICustomerContact | Record<PropertyKey, any> = {};

        if (form.contactId) {
            account.ContactId = form.contactId;
        }

        if (form.fio) {
            account.Fio = form.fio;
        }

        if (form.firstName) {
            account.FirstName = form.firstName;
        }
        if (form.lastName) {
            account.LastName = form.lastName;
        }
        if (form.patronymic) {
            account.Patronymic = form.patronymic;
        }

        if (form.country) {
            account.CountryId = form.country.CountryId;
            account.Country = form.country.Name;
        }

        if (form.region) {
            account.Region = form.region;
        }

        if (form.district) {
            account.District = form.district;
        }

        if (form.city) {
            account.City = form.city;
        }

        if (form.zip) {
            account.Zip = form.zip;
        }

        account.Street = form.street;
        account.House = form.house;
        account.Apartment = form.apartment;
        account.Structure = form.structure;
        account.Entrance = form.entrance;
        account.Floor = form.floor;

        account.IsShowName = this.isShowName;
        account.IsMain = this.addressSelected != null ? form.contactId === this.addressSelected.ContactId : false;

        return account;
    };
}
