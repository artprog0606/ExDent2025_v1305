namespace AdvantShop.Shipping.Dpd.Api.Calculator
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class ServiceCostFault : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string codeField;

        private city[] deliveryDupsField;

        private string messageField;

        private city[] pickupDupsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
                this.RaisePropertyChanged("code");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("deliveryDups", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public city[] deliveryDups
        {
            get
            {
                return this.deliveryDupsField;
            }
            set
            {
                this.deliveryDupsField = value;
                this.RaisePropertyChanged("deliveryDups");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
                this.RaisePropertyChanged("message");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("pickupDups", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public city[] pickupDups
        {
            get
            {
                return this.pickupDupsField;
            }
            set
            {
                this.pickupDupsField = value;
                this.RaisePropertyChanged("pickupDups");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class city : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long cityIdField;

        private bool cityIdFieldSpecified;

        private string countryCodeField;

        private string countryNameField;

        private int regionCodeField;

        private bool regionCodeFieldSpecified;

        private string regionNameField;

        private string cityNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public long cityId
        {
            get
            {
                return this.cityIdField;
            }
            set
            {
                this.cityIdField = value;
                this.RaisePropertyChanged("cityId");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cityIdSpecified
        {
            get
            {
                return this.cityIdFieldSpecified;
            }
            set
            {
                this.cityIdFieldSpecified = value;
                this.RaisePropertyChanged("cityIdSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string countryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
                this.RaisePropertyChanged("countryCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string countryName
        {
            get
            {
                return this.countryNameField;
            }
            set
            {
                this.countryNameField = value;
                this.RaisePropertyChanged("countryName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public int regionCode
        {
            get
            {
                return this.regionCodeField;
            }
            set
            {
                this.regionCodeField = value;
                this.RaisePropertyChanged("regionCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool regionCodeSpecified
        {
            get
            {
                return this.regionCodeFieldSpecified;
            }
            set
            {
                this.regionCodeFieldSpecified = value;
                this.RaisePropertyChanged("regionCodeSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string regionName
        {
            get
            {
                return this.regionNameField;
            }
            set
            {
                this.regionNameField = value;
                this.RaisePropertyChanged("regionName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string cityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
                this.RaisePropertyChanged("cityName");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class serviceCost : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string serviceCodeField;

        private string serviceNameField;

        private double costField;

        private bool costFieldSpecified;

        private int daysField;

        private bool daysFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string serviceCode
        {
            get
            {
                return this.serviceCodeField;
            }
            set
            {
                this.serviceCodeField = value;
                this.RaisePropertyChanged("serviceCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string serviceName
        {
            get
            {
                return this.serviceNameField;
            }
            set
            {
                this.serviceNameField = value;
                this.RaisePropertyChanged("serviceName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public double cost
        {
            get
            {
                return this.costField;
            }
            set
            {
                this.costField = value;
                this.RaisePropertyChanged("cost");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool costSpecified
        {
            get
            {
                return this.costFieldSpecified;
            }
            set
            {
                this.costFieldSpecified = value;
                this.RaisePropertyChanged("costSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public int days
        {
            get
            {
                return this.daysField;
            }
            set
            {
                this.daysField = value;
                this.RaisePropertyChanged("days");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool daysSpecified
        {
            get
            {
                return this.daysFieldSpecified;
            }
            set
            {
                this.daysFieldSpecified = value;
                this.RaisePropertyChanged("daysSpecified");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class cityRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long cityIdField;

        private bool cityIdFieldSpecified;

        private string indexField;

        private string cityNameField;

        private int regionCodeField;

        private bool regionCodeFieldSpecified;

        private string countryCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public long cityId
        {
            get
            {
                return this.cityIdField;
            }
            set
            {
                this.cityIdField = value;
                this.RaisePropertyChanged("cityId");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cityIdSpecified
        {
            get
            {
                return this.cityIdFieldSpecified;
            }
            set
            {
                this.cityIdFieldSpecified = value;
                this.RaisePropertyChanged("cityIdSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
                this.RaisePropertyChanged("index");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string cityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
                this.RaisePropertyChanged("cityName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public int regionCode
        {
            get
            {
                return this.regionCodeField;
            }
            set
            {
                this.regionCodeField = value;
                this.RaisePropertyChanged("regionCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool regionCodeSpecified
        {
            get
            {
                return this.regionCodeFieldSpecified;
            }
            set
            {
                this.regionCodeFieldSpecified = value;
                this.RaisePropertyChanged("regionCodeSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string countryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
                this.RaisePropertyChanged("countryCode");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class auth : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long clientNumberField;

        private string clientKeyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public long clientNumber
        {
            get
            {
                return this.clientNumberField;
            }
            set
            {
                this.clientNumberField = value;
                this.RaisePropertyChanged("clientNumber");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string clientKey
        {
            get
            {
                return this.clientKeyField;
            }
            set
            {
                this.clientKeyField = value;
                this.RaisePropertyChanged("clientKey");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class serviceCostRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private cityRequest pickupField;

        private cityRequest deliveryField;

        private bool selfPickupField;

        private bool selfDeliveryField;

        private double weightField;

        private double volumeField;

        private bool volumeFieldSpecified;

        private string serviceCodeField;

        private System.DateTime pickupDateField;

        private bool pickupDateFieldSpecified;

        private int maxDaysField;

        private bool maxDaysFieldSpecified;

        private double maxCostField;

        private bool maxCostFieldSpecified;

        private double declaredValueField;

        private bool declaredValueFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public auth auth
        {
            get
            {
                return this.authField;
            }
            set
            {
                this.authField = value;
                this.RaisePropertyChanged("auth");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public cityRequest pickup
        {
            get
            {
                return this.pickupField;
            }
            set
            {
                this.pickupField = value;
                this.RaisePropertyChanged("pickup");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public cityRequest delivery
        {
            get
            {
                return this.deliveryField;
            }
            set
            {
                this.deliveryField = value;
                this.RaisePropertyChanged("delivery");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public bool selfPickup
        {
            get
            {
                return this.selfPickupField;
            }
            set
            {
                this.selfPickupField = value;
                this.RaisePropertyChanged("selfPickup");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public bool selfDelivery
        {
            get
            {
                return this.selfDeliveryField;
            }
            set
            {
                this.selfDeliveryField = value;
                this.RaisePropertyChanged("selfDelivery");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public double weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
                this.RaisePropertyChanged("weight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public double volume
        {
            get
            {
                return this.volumeField;
            }
            set
            {
                this.volumeField = value;
                this.RaisePropertyChanged("volume");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool volumeSpecified
        {
            get
            {
                return this.volumeFieldSpecified;
            }
            set
            {
                this.volumeFieldSpecified = value;
                this.RaisePropertyChanged("volumeSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public string serviceCode
        {
            get
            {
                return this.serviceCodeField;
            }
            set
            {
                this.serviceCodeField = value;
                this.RaisePropertyChanged("serviceCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date", Order = 8)]
        public System.DateTime pickupDate
        {
            get
            {
                return this.pickupDateField;
            }
            set
            {
                this.pickupDateField = value;
                this.RaisePropertyChanged("pickupDate");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pickupDateSpecified
        {
            get
            {
                return this.pickupDateFieldSpecified;
            }
            set
            {
                this.pickupDateFieldSpecified = value;
                this.RaisePropertyChanged("pickupDateSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public int maxDays
        {
            get
            {
                return this.maxDaysField;
            }
            set
            {
                this.maxDaysField = value;
                this.RaisePropertyChanged("maxDays");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxDaysSpecified
        {
            get
            {
                return this.maxDaysFieldSpecified;
            }
            set
            {
                this.maxDaysFieldSpecified = value;
                this.RaisePropertyChanged("maxDaysSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 10)]
        public double maxCost
        {
            get
            {
                return this.maxCostField;
            }
            set
            {
                this.maxCostField = value;
                this.RaisePropertyChanged("maxCost");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxCostSpecified
        {
            get
            {
                return this.maxCostFieldSpecified;
            }
            set
            {
                this.maxCostFieldSpecified = value;
                this.RaisePropertyChanged("maxCostSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 11)]
        public double declaredValue
        {
            get
            {
                return this.declaredValueField;
            }
            set
            {
                this.declaredValueField = value;
                this.RaisePropertyChanged("declaredValue");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool declaredValueSpecified
        {
            get
            {
                return this.declaredValueFieldSpecified;
            }
            set
            {
                this.declaredValueFieldSpecified = value;
                this.RaisePropertyChanged("declaredValueSpecified");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class ServiceCostFault2 : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string codeField;

        private cityIndex[] deliveryDupsField;

        private string messageField;

        private cityIndex[] pickupDupsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
                this.RaisePropertyChanged("code");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("deliveryDups", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public cityIndex[] deliveryDups
        {
            get
            {
                return this.deliveryDupsField;
            }
            set
            {
                this.deliveryDupsField = value;
                this.RaisePropertyChanged("deliveryDups");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
                this.RaisePropertyChanged("message");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("pickupDups", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public cityIndex[] pickupDups
        {
            get
            {
                return this.pickupDupsField;
            }
            set
            {
                this.pickupDupsField = value;
                this.RaisePropertyChanged("pickupDups");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class cityIndex : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long cityIdField;

        private bool cityIdFieldSpecified;

        private string countryCodeField;

        private string countryNameField;

        private int regionCodeField;

        private bool regionCodeFieldSpecified;

        private string regionNameField;

        private string cityCodeField;

        private string cityNameField;

        private string abbreviationField;

        private string indexMinField;

        private string indexMaxField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public long cityId
        {
            get
            {
                return this.cityIdField;
            }
            set
            {
                this.cityIdField = value;
                this.RaisePropertyChanged("cityId");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cityIdSpecified
        {
            get
            {
                return this.cityIdFieldSpecified;
            }
            set
            {
                this.cityIdFieldSpecified = value;
                this.RaisePropertyChanged("cityIdSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string countryCode
        {
            get
            {
                return this.countryCodeField;
            }
            set
            {
                this.countryCodeField = value;
                this.RaisePropertyChanged("countryCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string countryName
        {
            get
            {
                return this.countryNameField;
            }
            set
            {
                this.countryNameField = value;
                this.RaisePropertyChanged("countryName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public int regionCode
        {
            get
            {
                return this.regionCodeField;
            }
            set
            {
                this.regionCodeField = value;
                this.RaisePropertyChanged("regionCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool regionCodeSpecified
        {
            get
            {
                return this.regionCodeFieldSpecified;
            }
            set
            {
                this.regionCodeFieldSpecified = value;
                this.RaisePropertyChanged("regionCodeSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string regionName
        {
            get
            {
                return this.regionNameField;
            }
            set
            {
                this.regionNameField = value;
                this.RaisePropertyChanged("regionName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string cityCode
        {
            get
            {
                return this.cityCodeField;
            }
            set
            {
                this.cityCodeField = value;
                this.RaisePropertyChanged("cityCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public string cityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
                this.RaisePropertyChanged("cityName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public string abbreviation
        {
            get
            {
                return this.abbreviationField;
            }
            set
            {
                this.abbreviationField = value;
                this.RaisePropertyChanged("abbreviation");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public string indexMin
        {
            get
            {
                return this.indexMinField;
            }
            set
            {
                this.indexMinField = value;
                this.RaisePropertyChanged("indexMin");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public string indexMax
        {
            get
            {
                return this.indexMaxField;
            }
            set
            {
                this.indexMaxField = value;
                this.RaisePropertyChanged("indexMax");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", ConfigurationName = "DpdCal.DPDCalculator")]
    public interface DPDCalculator
    {

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostRequest", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServiceCostFault), Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCost/Fault/Service" +
            "CostFault", Name = "ServiceCostFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getServiceCostResponse getServiceCost(getServiceCostRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostRequest", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostResponse")]
        System.Threading.Tasks.Task<getServiceCostResponse> getServiceCostAsync(getServiceCostRequest request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCost2Request", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCost2Response")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServiceCostFault2), Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCost2/Fault/Servic" +
            "eCostFault2", Name = "ServiceCostFault2")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getServiceCost2Response getServiceCost2(getServiceCost2Request request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCost2Request", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCost2Response")]
        System.Threading.Tasks.Task<getServiceCost2Response> getServiceCost2Async(getServiceCost2Request request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcelsReque" +
            "st", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcelsRespo" +
            "nse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServiceCostFault), Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcels/Faul" +
            "t/ServiceCostFault", Name = "ServiceCostFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getServiceCostByParcelsResponse getServiceCostByParcels(getServiceCostByParcelsRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcelsReque" +
            "st", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcelsRespo" +
            "nse")]
        System.Threading.Tasks.Task<getServiceCostByParcelsResponse> getServiceCostByParcelsAsync(getServiceCostByParcelsRequest request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcels2Requ" +
            "est", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcels2Resp" +
            "onse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServiceCostFault2), Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcels2/Fau" +
            "lt/ServiceCostFault2", Name = "ServiceCostFault2")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getServiceCostByParcels2Response getServiceCostByParcels2(getServiceCostByParcels2Request request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcels2Requ" +
            "est", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostByParcels2Resp" +
            "onse")]
        System.Threading.Tasks.Task<getServiceCostByParcels2Response> getServiceCostByParcels2Async(getServiceCostByParcels2Request request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostInternationalR" +
            "equest", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostInternationalR" +
            "esponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(ServiceCostFault), Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostInternational/" +
            "Fault/ServiceCostFault", Name = "ServiceCostFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getServiceCostInternationalResponse getServiceCostInternational(getServiceCostInternationalRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostInternationalR" +
            "equest", ReplyAction = "http://dpd.ru/ws/calculator/2012-03-20/DPDCalculator/getServiceCostInternationalR" +
            "esponse")]
        System.Threading.Tasks.Task<getServiceCostInternationalResponse> getServiceCostInternationalAsync(getServiceCostInternationalRequest request);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCost", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCostRequest request;

        public getServiceCostRequest()
        {
        }

        public getServiceCostRequest(serviceCostRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostResponse", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCost[] @return;

        public getServiceCostResponse()
        {
        }

        public getServiceCostResponse(serviceCost[] @return)
        {
            this.@return = @return;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCost2", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCost2Request
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCostRequest request;

        public getServiceCost2Request()
        {
        }

        public getServiceCost2Request(serviceCostRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCost2Response", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCost2Response
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCost[] @return;

        public getServiceCost2Response()
        {
        }

        public getServiceCost2Response(serviceCost[] @return)
        {
            this.@return = @return;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class serviceCostParcelsRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private cityRequest pickupField;

        private cityRequest deliveryField;

        private bool selfPickupField;

        private bool selfDeliveryField;

        private string serviceCodeField;

        private System.DateTime pickupDateField;

        private bool pickupDateFieldSpecified;

        private int maxDaysField;

        private bool maxDaysFieldSpecified;

        private double maxCostField;

        private bool maxCostFieldSpecified;

        private double declaredValueField;

        private bool declaredValueFieldSpecified;

        private parcelRequest[] parcelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public auth auth
        {
            get
            {
                return this.authField;
            }
            set
            {
                this.authField = value;
                this.RaisePropertyChanged("auth");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public cityRequest pickup
        {
            get
            {
                return this.pickupField;
            }
            set
            {
                this.pickupField = value;
                this.RaisePropertyChanged("pickup");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public cityRequest delivery
        {
            get
            {
                return this.deliveryField;
            }
            set
            {
                this.deliveryField = value;
                this.RaisePropertyChanged("delivery");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public bool selfPickup
        {
            get
            {
                return this.selfPickupField;
            }
            set
            {
                this.selfPickupField = value;
                this.RaisePropertyChanged("selfPickup");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public bool selfDelivery
        {
            get
            {
                return this.selfDeliveryField;
            }
            set
            {
                this.selfDeliveryField = value;
                this.RaisePropertyChanged("selfDelivery");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string serviceCode
        {
            get
            {
                return this.serviceCodeField;
            }
            set
            {
                this.serviceCodeField = value;
                this.RaisePropertyChanged("serviceCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, DataType = "date", Order = 6)]
        public System.DateTime pickupDate
        {
            get
            {
                return this.pickupDateField;
            }
            set
            {
                this.pickupDateField = value;
                this.RaisePropertyChanged("pickupDate");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool pickupDateSpecified
        {
            get
            {
                return this.pickupDateFieldSpecified;
            }
            set
            {
                this.pickupDateFieldSpecified = value;
                this.RaisePropertyChanged("pickupDateSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public int maxDays
        {
            get
            {
                return this.maxDaysField;
            }
            set
            {
                this.maxDaysField = value;
                this.RaisePropertyChanged("maxDays");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxDaysSpecified
        {
            get
            {
                return this.maxDaysFieldSpecified;
            }
            set
            {
                this.maxDaysFieldSpecified = value;
                this.RaisePropertyChanged("maxDaysSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public double maxCost
        {
            get
            {
                return this.maxCostField;
            }
            set
            {
                this.maxCostField = value;
                this.RaisePropertyChanged("maxCost");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxCostSpecified
        {
            get
            {
                return this.maxCostFieldSpecified;
            }
            set
            {
                this.maxCostFieldSpecified = value;
                this.RaisePropertyChanged("maxCostSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public double declaredValue
        {
            get
            {
                return this.declaredValueField;
            }
            set
            {
                this.declaredValueField = value;
                this.RaisePropertyChanged("declaredValue");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool declaredValueSpecified
        {
            get
            {
                return this.declaredValueFieldSpecified;
            }
            set
            {
                this.declaredValueFieldSpecified = value;
                this.RaisePropertyChanged("declaredValueSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parcel", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 10)]
        public parcelRequest[] parcel
        {
            get
            {
                return this.parcelField;
            }
            set
            {
                this.parcelField = value;
                this.RaisePropertyChanged("parcel");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class parcelRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private double weightField;

        private double lengthField;

        private double widthField;

        private double heightField;

        private int quantityField;

        public parcelRequest()
        {
            this.quantityField = 1;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public double weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
                this.RaisePropertyChanged("weight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public double length
        {
            get
            {
                return this.lengthField;
            }
            set
            {
                this.lengthField = value;
                this.RaisePropertyChanged("length");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public double width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
                this.RaisePropertyChanged("width");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public double height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
                this.RaisePropertyChanged("height");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        [System.ComponentModel.DefaultValueAttribute(1)]
        public int quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
                this.RaisePropertyChanged("quantity");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostByParcels", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostByParcelsRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCostParcelsRequest request;

        public getServiceCostByParcelsRequest()
        {
        }

        public getServiceCostByParcelsRequest(serviceCostParcelsRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostByParcelsResponse", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostByParcelsResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCost[] @return;

        public getServiceCostByParcelsResponse()
        {
        }

        public getServiceCostByParcelsResponse(serviceCost[] @return)
        {
            this.@return = @return;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostByParcels2", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostByParcels2Request
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCostParcelsRequest request;

        public getServiceCostByParcels2Request()
        {
        }

        public getServiceCostByParcels2Request(serviceCostParcelsRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostByParcels2Response", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostByParcels2Response
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCost[] @return;

        public getServiceCostByParcels2Response()
        {
        }

        public getServiceCostByParcels2Response(serviceCost[] @return)
        {
            this.@return = @return;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class serviceCostInternationalRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private cityInternationalRequest pickupField;

        private cityInternationalRequest deliveryField;

        private bool selfPickupField;

        private bool selfDeliveryField;

        private double weightField;

        private long lengthField;

        private long widthField;

        private long heightField;

        private double declaredValueField;

        private bool declaredValueFieldSpecified;

        private bool insuranceField;

        private bool insuranceFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public auth auth
        {
            get
            {
                return this.authField;
            }
            set
            {
                this.authField = value;
                this.RaisePropertyChanged("auth");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public cityInternationalRequest pickup
        {
            get
            {
                return this.pickupField;
            }
            set
            {
                this.pickupField = value;
                this.RaisePropertyChanged("pickup");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public cityInternationalRequest delivery
        {
            get
            {
                return this.deliveryField;
            }
            set
            {
                this.deliveryField = value;
                this.RaisePropertyChanged("delivery");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public bool selfPickup
        {
            get
            {
                return this.selfPickupField;
            }
            set
            {
                this.selfPickupField = value;
                this.RaisePropertyChanged("selfPickup");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public bool selfDelivery
        {
            get
            {
                return this.selfDeliveryField;
            }
            set
            {
                this.selfDeliveryField = value;
                this.RaisePropertyChanged("selfDelivery");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public double weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
                this.RaisePropertyChanged("weight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public long length
        {
            get
            {
                return this.lengthField;
            }
            set
            {
                this.lengthField = value;
                this.RaisePropertyChanged("length");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public long width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
                this.RaisePropertyChanged("width");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public long height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
                this.RaisePropertyChanged("height");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public double declaredValue
        {
            get
            {
                return this.declaredValueField;
            }
            set
            {
                this.declaredValueField = value;
                this.RaisePropertyChanged("declaredValue");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool declaredValueSpecified
        {
            get
            {
                return this.declaredValueFieldSpecified;
            }
            set
            {
                this.declaredValueFieldSpecified = value;
                this.RaisePropertyChanged("declaredValueSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 10)]
        public bool insurance
        {
            get
            {
                return this.insuranceField;
            }
            set
            {
                this.insuranceField = value;
                this.RaisePropertyChanged("insurance");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool insuranceSpecified
        {
            get
            {
                return this.insuranceFieldSpecified;
            }
            set
            {
                this.insuranceFieldSpecified = value;
                this.RaisePropertyChanged("insuranceSpecified");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class cityInternationalRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string countryNameField;

        private string cityNameField;

        private long cityIdField;

        private bool cityIdFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string countryName
        {
            get
            {
                return this.countryNameField;
            }
            set
            {
                this.countryNameField = value;
                this.RaisePropertyChanged("countryName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string cityName
        {
            get
            {
                return this.cityNameField;
            }
            set
            {
                this.cityNameField = value;
                this.RaisePropertyChanged("cityName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public long cityId
        {
            get
            {
                return this.cityIdField;
            }
            set
            {
                this.cityIdField = value;
                this.RaisePropertyChanged("cityId");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool cityIdSpecified
        {
            get
            {
                return this.cityIdFieldSpecified;
            }
            set
            {
                this.cityIdFieldSpecified = value;
                this.RaisePropertyChanged("cityIdSpecified");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20")]
    public partial class serviceCostInternational : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string serviceCodeField;

        private string serviceNameField;

        private string daysField;

        private double costField;

        private bool costFieldSpecified;

        private double costPinField;

        private bool costPinFieldSpecified;

        private double weightField;

        private bool weightFieldSpecified;

        private double volumeField;

        private bool volumeFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string serviceCode
        {
            get
            {
                return this.serviceCodeField;
            }
            set
            {
                this.serviceCodeField = value;
                this.RaisePropertyChanged("serviceCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string serviceName
        {
            get
            {
                return this.serviceNameField;
            }
            set
            {
                this.serviceNameField = value;
                this.RaisePropertyChanged("serviceName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string days
        {
            get
            {
                return this.daysField;
            }
            set
            {
                this.daysField = value;
                this.RaisePropertyChanged("days");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public double cost
        {
            get
            {
                return this.costField;
            }
            set
            {
                this.costField = value;
                this.RaisePropertyChanged("cost");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool costSpecified
        {
            get
            {
                return this.costFieldSpecified;
            }
            set
            {
                this.costFieldSpecified = value;
                this.RaisePropertyChanged("costSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public double costPin
        {
            get
            {
                return this.costPinField;
            }
            set
            {
                this.costPinField = value;
                this.RaisePropertyChanged("costPin");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool costPinSpecified
        {
            get
            {
                return this.costPinFieldSpecified;
            }
            set
            {
                this.costPinFieldSpecified = value;
                this.RaisePropertyChanged("costPinSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public double weight
        {
            get
            {
                return this.weightField;
            }
            set
            {
                this.weightField = value;
                this.RaisePropertyChanged("weight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool weightSpecified
        {
            get
            {
                return this.weightFieldSpecified;
            }
            set
            {
                this.weightFieldSpecified = value;
                this.RaisePropertyChanged("weightSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public double volume
        {
            get
            {
                return this.volumeField;
            }
            set
            {
                this.volumeField = value;
                this.RaisePropertyChanged("volume");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool volumeSpecified
        {
            get
            {
                return this.volumeFieldSpecified;
            }
            set
            {
                this.volumeFieldSpecified = value;
                this.RaisePropertyChanged("volumeSpecified");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostInternational", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostInternationalRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCostInternationalRequest request;

        public getServiceCostInternationalRequest()
        {
        }

        public getServiceCostInternationalRequest(serviceCostInternationalRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getServiceCostInternationalResponse", WrapperNamespace = "http://dpd.ru/ws/calculator/2012-03-20", IsWrapped = true)]
    public partial class getServiceCostInternationalResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/calculator/2012-03-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public serviceCostInternational[] @return;

        public getServiceCostInternationalResponse()
        {
        }

        public getServiceCostInternationalResponse(serviceCostInternational[] @return)
        {
            this.@return = @return;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface DPDCalculatorChannel : DPDCalculator, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DPDCalculatorClient : System.ServiceModel.ClientBase<DPDCalculator>, DPDCalculator
    {

        public DPDCalculatorClient()
        {
        }

        public DPDCalculatorClient(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public DPDCalculatorClient(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public DPDCalculatorClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public DPDCalculatorClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getServiceCostResponse DPDCalculator.getServiceCost(getServiceCostRequest request)
        {
            return base.Channel.getServiceCost(request);
        }

        public serviceCost[] getServiceCost(serviceCostRequest request)
        {
            getServiceCostRequest inValue = new getServiceCostRequest();
            inValue.request = request;
            getServiceCostResponse retVal = ((DPDCalculator)(this)).getServiceCost(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getServiceCostResponse> DPDCalculator.getServiceCostAsync(getServiceCostRequest request)
        {
            return base.Channel.getServiceCostAsync(request);
        }

        public System.Threading.Tasks.Task<getServiceCostResponse> getServiceCostAsync(serviceCostRequest request)
        {
            getServiceCostRequest inValue = new getServiceCostRequest();
            inValue.request = request;
            return ((DPDCalculator)(this)).getServiceCostAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getServiceCost2Response DPDCalculator.getServiceCost2(getServiceCost2Request request)
        {
            return base.Channel.getServiceCost2(request);
        }

        public serviceCost[] getServiceCost2(serviceCostRequest request)
        {
            getServiceCost2Request inValue = new getServiceCost2Request();
            inValue.request = request;
            getServiceCost2Response retVal = ((DPDCalculator)(this)).getServiceCost2(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getServiceCost2Response> DPDCalculator.getServiceCost2Async(getServiceCost2Request request)
        {
            return base.Channel.getServiceCost2Async(request);
        }

        public System.Threading.Tasks.Task<getServiceCost2Response> getServiceCost2Async(serviceCostRequest request)
        {
            getServiceCost2Request inValue = new getServiceCost2Request();
            inValue.request = request;
            return ((DPDCalculator)(this)).getServiceCost2Async(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getServiceCostByParcelsResponse DPDCalculator.getServiceCostByParcels(getServiceCostByParcelsRequest request)
        {
            return base.Channel.getServiceCostByParcels(request);
        }

        public serviceCost[] getServiceCostByParcels(serviceCostParcelsRequest request)
        {
            getServiceCostByParcelsRequest inValue = new getServiceCostByParcelsRequest();
            inValue.request = request;
            getServiceCostByParcelsResponse retVal = ((DPDCalculator)(this)).getServiceCostByParcels(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getServiceCostByParcelsResponse> DPDCalculator.getServiceCostByParcelsAsync(getServiceCostByParcelsRequest request)
        {
            return base.Channel.getServiceCostByParcelsAsync(request);
        }

        public System.Threading.Tasks.Task<getServiceCostByParcelsResponse> getServiceCostByParcelsAsync(serviceCostParcelsRequest request)
        {
            getServiceCostByParcelsRequest inValue = new getServiceCostByParcelsRequest();
            inValue.request = request;
            return ((DPDCalculator)(this)).getServiceCostByParcelsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getServiceCostByParcels2Response DPDCalculator.getServiceCostByParcels2(getServiceCostByParcels2Request request)
        {
            return base.Channel.getServiceCostByParcels2(request);
        }

        public serviceCost[] getServiceCostByParcels2(serviceCostParcelsRequest request)
        {
            getServiceCostByParcels2Request inValue = new getServiceCostByParcels2Request();
            inValue.request = request;
            getServiceCostByParcels2Response retVal = ((DPDCalculator)(this)).getServiceCostByParcels2(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getServiceCostByParcels2Response> DPDCalculator.getServiceCostByParcels2Async(getServiceCostByParcels2Request request)
        {
            return base.Channel.getServiceCostByParcels2Async(request);
        }

        public System.Threading.Tasks.Task<getServiceCostByParcels2Response> getServiceCostByParcels2Async(serviceCostParcelsRequest request)
        {
            getServiceCostByParcels2Request inValue = new getServiceCostByParcels2Request();
            inValue.request = request;
            return ((DPDCalculator)(this)).getServiceCostByParcels2Async(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getServiceCostInternationalResponse DPDCalculator.getServiceCostInternational(getServiceCostInternationalRequest request)
        {
            return base.Channel.getServiceCostInternational(request);
        }

        public serviceCostInternational[] getServiceCostInternational(serviceCostInternationalRequest request)
        {
            getServiceCostInternationalRequest inValue = new getServiceCostInternationalRequest();
            inValue.request = request;
            getServiceCostInternationalResponse retVal = ((DPDCalculator)(this)).getServiceCostInternational(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getServiceCostInternationalResponse> DPDCalculator.getServiceCostInternationalAsync(getServiceCostInternationalRequest request)
        {
            return base.Channel.getServiceCostInternationalAsync(request);
        }

        public System.Threading.Tasks.Task<getServiceCostInternationalResponse> getServiceCostInternationalAsync(serviceCostInternationalRequest request)
        {
            getServiceCostInternationalRequest inValue = new getServiceCostInternationalRequest();
            inValue.request = request;
            return ((DPDCalculator)(this)).getServiceCostInternationalAsync(inValue);
        }
    }
}
