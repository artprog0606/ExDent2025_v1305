import { PubSub } from '../../_common/PubSub/PubSub.js';

class RegistrationPageCtrl {
    /*@ngInject*/
    constructor($http, $q, $timeout, $translate, $window, toaster) {
        this.$http = $http;
        this.$q = $q;
        this.$timeout = $timeout;
        this.$translate = $translate;
        this.toaster = toaster;
        this.$window = $window;
        this.processCompanyNameTimer = null;
    }

    submit() {
        this.checkCaptcha()
            .then(() =>
                this.$http.post('user/registrationjson', {
                    firstName: this.fname,
                    lastname: this.lastname,
                    patronymic: this.patronymic,
                    email: this.email,
                    phone: this.phone,
                    birthday: this.birthday,
                    password: this.pass,
                    passwordConfirm: this.passagain,
                    wantBonusCard: this.wantBonusCard,
                    newsSubscription: this.subscr,
                    agree: this.agreement,
                    customerFields: this.CustomerFields,
                    lpId: this.lpId,
                    customerType: this.customerType,
                    userAgreementForPromotionalNewsletter: this.userAgreementForPromotionalNewsletter,
                }),
            )
            .then((response) => {
                if (response.data.result === true) {
                    PubSub.publish('user.registration');
                    setTimeout(() => this.$window.location.assign(response.data.obj), 300);
                    return response.data;
                }
                return this.$q.reject(response.data.errors);
            })
            .catch((result) => {
                this.toaster.pop('error', '', Array.isArray(result) ? result.join('<br>') : result);
            });
    }
    checkCaptcha() {
        if (typeof CaptchaSource !== 'undefined') {
            CaptchaSource.InputId = 'CaptchaCode';

            const input = CaptchaSource.GetInputElement();
            if (typeof input === 'undefined' || input === null) {
                return this.$q.resolve();
            }

            return this.$http.get(`${CaptchaSource.ValidationUrl}&i=${input.value}`).then((result) => {
                this.$timeout(() => {
                    CaptchaSource.ReloadImage();
                }, 1000);
                CaptchaSource.GetInputElement().value = '';

                if (result.data === true) {
                    return this.$q.resolve();
                }
                return this.$q.reject(this.$translate.instant('Js.Captcha.Wrong'));
            });
        }
        return this.$q.resolve();
    }

    processCompanyName(item) {
        if (this.processCompanyNameTimer !== null) {
            this.$timeout.cancel(this.processCompanyNameTimer);
        }
        const ctrl = this;
        return (this.processCompanyNameTimer = this.$timeout(
            () => {
                if (typeof item !== 'undefined' && item !== null && item.CompanyData) {
                    ctrl.CustomerFields.forEach((field) => {
                        if (field.FieldAssignment === 1) field.Value = item.CompanyData.CompanyName;
                        else if (field.FieldAssignment === 2) field.Value = item.CompanyData.LegalAddress;
                        else if (field.FieldAssignment === 3) field.Value = item.CompanyData.INN;
                        else if (field.FieldAssignment === 4) field.Value = item.CompanyData.KPP;
                        else if (field.FieldAssignment === 5) field.Value = item.CompanyData.OGRN;
                        else if (field.FieldAssignment === 6) field.Value = item.CompanyData.OKPO;
                    });
                } else if (typeof item !== 'undefined' && item !== null && item.BankData) {
                    ctrl.CustomerFields.forEach((field) => {
                        if (field.FieldAssignment === 7) field.Value = item.BankData.BIK;
                        else if (field.FieldAssignment === 8) field.Value = item.BankData.BankName;
                        else if (field.FieldAssignment === 9) field.Value = item.BankData.CorrespondentAccount;
                    });
                }
            },
            typeof item !== 'undefined' && item !== null ? 0 : 700,
        ));
    }
}

export default RegistrationPageCtrl;
