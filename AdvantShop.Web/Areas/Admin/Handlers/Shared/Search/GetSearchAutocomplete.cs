using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Web.Admin.Models.Shared.Search;
using AdvantShop.Web.Infrastructure.Extensions;
using Debug = AdvantShop.Diagnostics.Debug;

namespace AdvantShop.Web.Admin.Handlers.Shared.Search
{
    public class GetSearchAutocomplete
    {
        private readonly string _type;
        private readonly string _q;
        private readonly UrlHelper _url;
        private int _itemsCount = 5;

        public GetSearchAutocomplete(string q, string type)
        {
            _type = type;
            _q = HttpUtility.HtmlEncode(q);
            _url = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public List<SearchAutocompleteModel> Execute()
        {
            var model = new List<SearchAutocompleteModel>();

            try
            {
                if (!string.IsNullOrEmpty(_type))
                {
                    _itemsCount = 15;

                    switch (_type)
                    {
                        case "settings":
                            if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Settings))
                            {
                                model.Add(GetSettings(_q));
                            }
                            break;
                        case "categories":
                            if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Catalog))
                            {
                                model.Add(GetCategories(_q));
                            }
                            break;
                        case "products":
                            if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Catalog))
                            {
                                model.Add(GetProducts(_q));
                            }
                            break;
                        case "customers":
                            if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Customers))
                            {
                                model.Add(GetCustomers(_q));
                            }
                            break;
                        case "orders":
                            if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Orders))
                            {
                                model.Add(GetOrders(_q));
                            }
                            break;
                        case "leads":
                            if (SettingsCrm.CrmActive && RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Crm))
                            {
                                model.Add(GetLeads(_q));
                            }
                            break;
                        case "tasks":
                            if (SettingsTasks.TasksActive && RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Tasks))
                            {
                                model.Add(GetTasks(_q));
                            }
                            break;
                        case "modules":
                            if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Modules))
                            {
                                model.Add(GetModules(_q));
                            }
                            break;
                    }
                }
                else
                {
                    var searchTasks = new List<Task<List<SearchAutocompleteModel>>>();

                    if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Settings))
                    {
                        searchTasks.Add(GetAsync(GetSettings, _q));
                    }

                    if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Catalog))
                    {
                        searchTasks.Add(GetAsync(GetCategories, _q));
                    }

                    if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Catalog))
                    {
                        searchTasks.Add(GetAsync(GetProducts, _q));
                    }

                    if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Customers))
                    {
                        searchTasks.Add(GetAsync(GetCustomers, _q));
                    }

                    if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Orders))
                    {
                        searchTasks.Add(GetAsync(GetOrders, _q));
                    }
                    
                    if (!Saas.SaasDataService.IsSaasEnabled || Saas.SaasDataService.CurrentSaasData.HaveCrm)
                    {
                        if (SettingsCrm.CrmActive && RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Crm))
                        {
                            searchTasks.Add(GetAsync(GetLeads, _q));
                        }
                        if (SettingsTasks.TasksActive && RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Tasks))
                        {
                            searchTasks.Add(GetAsync(GetTasks, _q));
                        }
                    }

                    if (RoleActionService.HasCurrentCustomerRoleAction(RoleAction.Modules))
                    {
                        searchTasks.Add(GetAsync(GetModules, _q));
                    }
                                        

                    model = searchTasks.Select(x => x.Result).SelectMany(x => x).Where(x => x != null).ToList();

                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return model.Where(x => x.Items.Any()).ToList();
        }

        private Task<List<SearchAutocompleteModel>> GetAsync(Func<string, SearchAutocompleteModel> action, string param)
        {
            var context = HttpContext.Current;
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                HttpContext.Current = context;
                Localization.Culture.InitializeCulture();

                return new List<SearchAutocompleteModel> {action(param)};
            });
        }

        private SearchAutocompleteModel GetProducts(string q)
        {
            var watch = Stopwatch.StartNew();

            var tanslitQ = StringHelper.TranslitToRusKeyboard(q);
            var productIds =
                ProductSearcherAdmin.Search(q)
                    .SearchResultItems.Select(item => item.Id)
                    .Union(ProductSearcherAdmin.Search(tanslitQ).SearchResultItems.Select(item => item.Id))
                    .Distinct()
                    .ToList();

            var products = productIds.Take(_itemsCount).Select(ProductService.GetProduct).Where(x => x != null).ToList();

            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Products"),
                Items = products.Select(product => new SearchAutocompleteItem()
                {
                    Text = product.Name,
                    Description = LocalizationService.GetResourceFormat("Admin.SearchAutocomplete.ProductsDescFormat", product.ArtNo),
                    Url = _url.AbsoluteActionUrl("Edit", "Product", new { id = product.ProductId })
                }).ToList(),
            };

            if (productIds.Count > _itemsCount)
            {
                model.More = new SearchAutocompleteItem()
                {
                    Text = LocalizationService.GetResourceFormat("Admin.SearchAutocomplete.AllResultsFormat", productIds.Count()),
                    Url = _url.AbsoluteActionUrl("Index", "Catalog", new { showMethod = "AllProducts", search = q, from = "search" })
                };
            }

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }

        private SearchAutocompleteModel GetCategories(string q)
        {
            var watch = Stopwatch.StartNew();

            var tanslitQ = StringHelper.TranslitToRusKeyboard(q);
            var categoryIds =
                CategorySearcherAdmin.Search(q)
                    .SearchResultItems.Select(item => item.Id)
                    .Union(CategorySearcherAdmin.Search(tanslitQ).SearchResultItems.Select(item => item.Id))
                    .Distinct()
                    .ToList();

            var categories = categoryIds.Take(_itemsCount).Select(CategoryService.GetCategory).Where(x => x != null).ToList();


            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Categories"),
                Items = categories.Select(category => new SearchAutocompleteItem()
                {
                    Text = (category.ParentCategory != null && category.ID != 0 ? category.ParentCategory.Name + " > " : "") + category.Name,
                    Url = _url.AbsoluteActionUrl("Edit", "Category", new { id = category.CategoryId })
                }).ToList()
            };


            if (categoryIds.Count > _itemsCount)
            {
                model.More = new SearchAutocompleteItem()
                {
                    Text = LocalizationService.GetResourceFormat("Admin.SearchAutocomplete.AllResultsFormat", categoryIds.Count()),
                    Url = _url.AbsoluteActionUrl("Index", "Catalog", new { categorySearch = q })
                };
            }


            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }

        private SearchAutocompleteModel GetLeads(string q)
        {
            var watch = Stopwatch.StartNew();

            var leads = new List<Lead>();

            foreach (var word in q.Split(" "))
                leads.AddRange(LeadService.GetLeadsForAutocomplete(word).Where(x => !leads.Any(l => l.Id == x.Id)));

            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Leads"),
                Items = leads.Take(_itemsCount).Select(lead => new SearchAutocompleteItem()
                {
                    Text = "№" + lead.Id + ", " + (lead.DealStatus != null ? lead.DealStatus.Name : ""),
                    Url = _url.AbsoluteActionUrl("Index", "Leads") + "?leadIdInfo=" + lead.Id
                }).ToList()
            };

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }

        private SearchAutocompleteModel GetTasks(string q)
        {
            var watch = Stopwatch.StartNew();

            var tasks = new List<Core.Services.Crm.Task>();

            foreach (var word in q.Split(" "))
                tasks.AddRange(TaskService.GetTasksForAutocomplete(word).Where(x => !tasks.Any(t => t.Id == x.Id)));
            
            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Tasks"),
                Items = tasks.Take(_itemsCount).Select(task => new SearchAutocompleteItem()
                {
                    Text = "№" + task.Id + ", " + task.Name,
                    Url = UrlService.GetAdminUrl("tasks?modal=" + task.Id)
                }).ToList()
            };

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }

        private SearchAutocompleteModel GetModules(string q)
        {
            var watch = Stopwatch.StartNew();

            var tanslitQ = StringHelper.TranslitToRusKeyboard(q);

            var modules = ModulesRepository.GetModulesFromDb().Where(m => m.StringId.Contains(q) || (m.Name != null && (m.Name.Contains(q) || m.Name.Contains(tanslitQ)))).Take(_itemsCount);
            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Modules"),
                Items = modules.Select(module => new SearchAutocompleteItem()
                {
                    Text = module.Name,
                    Url = _url.AbsoluteActionUrl("Details", "Modules", new { id = module.StringId })
                }).ToList()
            };

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }

        private SearchAutocompleteModel GetOrders(string q)
        {
            var watch = Stopwatch.StartNew();

            var orders = new List<OrderAutocomplete>();

            foreach (var word in q.Split(" "))
            {
                orders.AddRange(OrderService.GetOrdersForAutocomplete(word).Where(order => !orders.Any(o => o.OrderID == order.OrderID)));
            }

            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Orders"),
                Items = orders.Take(_itemsCount).Select(order => new SearchAutocompleteItem()
                {
                    Text = "№" + order.Number + ", " + order.StatusName,
                    Url = _url.AbsoluteActionUrl("Edit", "Orders", new { id = order.OrderID })
                }).ToList()
            };

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }


        private SearchAutocompleteModel GetSettings(string q)
        {
            var watch = Stopwatch.StartNew();

            var settings = SettingsSearchService.GetSettingsSearchForAutocomplete(q).Take(_itemsCount);
            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Settings"),
                Items = settings.Select(setting => new SearchAutocompleteItem()
                {
                    Text = setting.Title,
                    Url = setting.Link
                }).ToList()
            };

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }


        private SearchAutocompleteModel GetCustomers(string q)
        {
            var watch = Stopwatch.StartNew();

            var model = new SearchAutocompleteModel()
            {
                Category = LocalizationService.GetResource("Admin.SearchAutocomplete.Customers"),
                Items = new List<SearchAutocompleteItem>()
            };

            if (_type != "customers")
            {
                var customersByClientCode = ClientCodeService.SearchCustomers(q).Take(_itemsCount).ToList();
                if (customersByClientCode.Count > 0)
                {
                    model.Items.AddRange(customersByClientCode.Select(customer => new SearchAutocompleteItem()
                    {
                        Text = new[] { customer.Code.ToString(), (customer.FirstName + " " + customer.LastName).Trim(), customer.Phone }.Where(str => str.IsNotEmpty()).AggregateString(", "),
                        Url = _url.AbsoluteActionUrl("View", "Customers", new { id = customer.Id != Guid.Empty ? customer.Id : (Guid?)null, code = customer.Code })
                    }));
                }
            }

            var customers = new List<Customer>();

            foreach (var word in q.Split(" "))
            {
                customers.AddRange(CustomerService.GetCustomersForAutocomplete(word, _itemsCount).Where(x => !customers.Any(c => c.Id == x.Id)));
            }


            model.Items.AddRange(customers.Take(_itemsCount).Select(customer => new SearchAutocompleteItem()
            {
                Text = new[] { (customer.FirstName + " " + customer.LastName).Trim(), customer.EMail, customer.Phone }.Where(str => str.IsNotEmpty()).AggregateString(", "),
                Url = _url.AbsoluteActionUrl("View", "Customers", new { id = customer.Id }),
                Id = customer.Id.ToString()
            }));

            watch.Stop();
            model.Time = watch.ElapsedMilliseconds;
            return model;
        }
    }
}
