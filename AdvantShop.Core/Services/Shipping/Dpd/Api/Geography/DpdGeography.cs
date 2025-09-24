namespace AdvantShop.Shipping.Dpd.Api.Geography
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class WSFault : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string codeField;

        private string messageField;

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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class extraServiceParam : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
                this.RaisePropertyChanged("name");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
                this.RaisePropertyChanged("value");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class extraService : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string esCodeField;

        private extraServiceParam[] paramsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string esCode
        {
            get
            {
                return this.esCodeField;
            }
            set
            {
                this.esCodeField = value;
                this.RaisePropertyChanged("esCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("params", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public extraServiceParam[] @params
        {
            get
            {
                return this.paramsField;
            }
            set
            {
                this.paramsField = value;
                this.RaisePropertyChanged("params");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class timetable : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string weekDaysField;

        private string workTimeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string weekDays
        {
            get
            {
                return this.weekDaysField;
            }
            set
            {
                this.weekDaysField = value;
                this.RaisePropertyChanged("weekDays");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string workTime
        {
            get
            {
                return this.workTimeField;
            }
            set
            {
                this.workTimeField = value;
                this.RaisePropertyChanged("workTime");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class schedule : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string operationField;

        private timetable[] timetableField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string operation
        {
            get
            {
                return this.operationField;
            }
            set
            {
                this.operationField = value;
                this.RaisePropertyChanged("operation");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("timetable", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public timetable[] timetable
        {
            get
            {
                return this.timetableField;
            }
            set
            {
                this.timetableField = value;
                this.RaisePropertyChanged("timetable");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class geoCoordinates : object, System.ComponentModel.INotifyPropertyChanged
    {

        private decimal latitudeField;

        private bool latitudeFieldSpecified;

        private decimal longitudeField;

        private bool longitudeFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public decimal latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
                this.RaisePropertyChanged("latitude");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool latitudeSpecified
        {
            get
            {
                return this.latitudeFieldSpecified;
            }
            set
            {
                this.latitudeFieldSpecified = value;
                this.RaisePropertyChanged("latitudeSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public decimal longitude
        {
            get
            {
                return this.longitudeField;
            }
            set
            {
                this.longitudeField = value;
                this.RaisePropertyChanged("longitude");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool longitudeSpecified
        {
            get
            {
                return this.longitudeFieldSpecified;
            }
            set
            {
                this.longitudeFieldSpecified = value;
                this.RaisePropertyChanged("longitudeSpecified");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class address : object, System.ComponentModel.INotifyPropertyChanged
    {

        private long cityIdField;

        private bool cityIdFieldSpecified;

        private string countryCodeField;

        private string regionCodeField;

        private string regionNameField;

        private string cityCodeField;

        private string cityNameField;

        private string indexField;

        private string streetField;

        private string streetAbbrField;

        private string houseNoField;

        private string buildingField;

        private string structureField;

        private string ownershipField;

        private string descriptField;

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
        public string regionCode
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public string street
        {
            get
            {
                return this.streetField;
            }
            set
            {
                this.streetField = value;
                this.RaisePropertyChanged("street");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public string streetAbbr
        {
            get
            {
                return this.streetAbbrField;
            }
            set
            {
                this.streetAbbrField = value;
                this.RaisePropertyChanged("streetAbbr");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public string houseNo
        {
            get
            {
                return this.houseNoField;
            }
            set
            {
                this.houseNoField = value;
                this.RaisePropertyChanged("houseNo");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 10)]
        public string building
        {
            get
            {
                return this.buildingField;
            }
            set
            {
                this.buildingField = value;
                this.RaisePropertyChanged("building");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 11)]
        public string structure
        {
            get
            {
                return this.structureField;
            }
            set
            {
                this.structureField = value;
                this.RaisePropertyChanged("structure");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 12)]
        public string ownership
        {
            get
            {
                return this.ownershipField;
            }
            set
            {
                this.ownershipField = value;
                this.RaisePropertyChanged("ownership");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 13)]
        public string descript
        {
            get
            {
                return this.descriptField;
            }
            set
            {
                this.descriptField = value;
                this.RaisePropertyChanged("descript");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class terminalSelf : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string terminalCodeField;

        private string terminalNameField;

        private address addressField;

        private geoCoordinates geoCoordinatesField;

        private schedule[] scheduleField;

        private extraService[] extraServiceField;

        private string[] servicesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string terminalCode
        {
            get
            {
                return this.terminalCodeField;
            }
            set
            {
                this.terminalCodeField = value;
                this.RaisePropertyChanged("terminalCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string terminalName
        {
            get
            {
                return this.terminalNameField;
            }
            set
            {
                this.terminalNameField = value;
                this.RaisePropertyChanged("terminalName");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public address address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
                this.RaisePropertyChanged("address");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public geoCoordinates geoCoordinates
        {
            get
            {
                return this.geoCoordinatesField;
            }
            set
            {
                this.geoCoordinatesField = value;
                this.RaisePropertyChanged("geoCoordinates");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("schedule", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public schedule[] schedule
        {
            get
            {
                return this.scheduleField;
            }
            set
            {
                this.scheduleField = value;
                this.RaisePropertyChanged("schedule");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("extraService", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public extraService[] extraService
        {
            get
            {
                return this.extraServiceField;
            }
            set
            {
                this.extraServiceField = value;
                this.RaisePropertyChanged("extraService");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        [System.Xml.Serialization.XmlArrayItemAttribute("serviceCode", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] services
        {
            get
            {
                return this.servicesField;
            }
            set
            {
                this.servicesField = value;
                this.RaisePropertyChanged("services");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", ConfigurationName = "Dpd.DPDGeography2")]
    public interface DPDGeography2
    {

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlArrayAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getTerminalsSelfDelivery2Requ" +
            "est", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getTerminalsSelfDelivery2Resp" +
            "onse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WSFault), Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getTerminalsSelfDelivery2/Fau" +
            "lt/WSFault", Name = "WSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getTerminalsSelfDelivery2Response getTerminalsSelfDelivery2(getTerminalsSelfDelivery2Request request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getTerminalsSelfDelivery2Requ" +
            "est", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getTerminalsSelfDelivery2Resp" +
            "onse")]
        System.Threading.Tasks.Task<getTerminalsSelfDelivery2Response> getTerminalsSelfDelivery2Async(getTerminalsSelfDelivery2Request request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getPossibleExtraServiceReques" +
            "t", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getPossibleExtraServiceRespon" +
            "se")]
        [System.ServiceModel.FaultContractAttribute(typeof(WSFault), Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getPossibleExtraService/Fault" +
            "/WSFault", Name = "WSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getPossibleExtraServiceResponse getPossibleExtraService(getPossibleExtraServiceRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getPossibleExtraServiceReques" +
            "t", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getPossibleExtraServiceRespon" +
            "se")]
        System.Threading.Tasks.Task<getPossibleExtraServiceResponse> getPossibleExtraServiceAsync(getPossibleExtraServiceRequest request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlElementAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getCitiesCashPayRequest", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getCitiesCashPayResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WSFault), Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getCitiesCashPay/Fault/WSFaul" +
            "t", Name = "WSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getCitiesCashPayResponse getCitiesCashPay(getCitiesCashPayRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getCitiesCashPayRequest", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getCitiesCashPayResponse")]
        System.Threading.Tasks.Task<getCitiesCashPayResponse> getCitiesCashPayAsync(getCitiesCashPayRequest request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlArrayAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getParcelShopsRequest", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getParcelShopsResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WSFault), Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getParcelShops/Fault/WSFault", Name = "WSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getParcelShopsResponse getParcelShops(getParcelShopsRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getParcelShopsRequest", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getParcelShopsResponse")]
        System.Threading.Tasks.Task<getParcelShopsResponse> getParcelShopsAsync(getParcelShopsRequest request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'System.Xml.Serialization.XmlArrayAttribute'.
        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getStoragePeriodRequest", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getStoragePeriodResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(WSFault), Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getStoragePeriod/Fault/WSFaul" +
            "t", Name = "WSFault")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults = true)]
        [return: System.ServiceModel.MessageParameterAttribute(Name = "return")]
        getStoragePeriodResponse getStoragePeriod(getStoragePeriodRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getStoragePeriodRequest", ReplyAction = "http://dpd.ru/ws/geography/2015-05-20/DPDGeography2/getStoragePeriodResponse")]
        System.Threading.Tasks.Task<getStoragePeriodResponse> getStoragePeriodAsync(getStoragePeriodRequest request);
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getTerminalsSelfDelivery2", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getTerminalsSelfDelivery2Request
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public auth auth;

        public getTerminalsSelfDelivery2Request()
        {
        }

        public getTerminalsSelfDelivery2Request(auth auth)
        {
            this.auth = auth;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getTerminalsSelfDelivery2Response", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getTerminalsSelfDelivery2Response
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("terminal", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public terminalSelf[] @return;

        public getTerminalsSelfDelivery2Response()
        {
        }

        public getTerminalsSelfDelivery2Response(terminalSelf[] @return)
        {
            this.@return = @return;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class dpdPossibleESRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private dpdPossibleESPickupDelivery pickupField;

        private dpdPossibleESPickupDelivery deliveryField;

        private bool selfPickupField;

        private bool selfDeliveryField;

        private string serviceCodeField;

        private System.DateTime pickupDateField;

        private bool pickupDateFieldSpecified;

        private string[] optionsField;

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
        public dpdPossibleESPickupDelivery pickup
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
        public dpdPossibleESPickupDelivery delivery
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
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        [System.Xml.Serialization.XmlArrayItemAttribute("option", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] options
        {
            get
            {
                return this.optionsField;
            }
            set
            {
                this.optionsField = value;
                this.RaisePropertyChanged("options");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class dpdPossibleESPickupDelivery : object, System.ComponentModel.INotifyPropertyChanged
    {

        private decimal cityIdField;

        private bool cityIdFieldSpecified;

        private string terminalCodeField;

        private decimal indexField;

        private bool indexFieldSpecified;

        private string cityNameField;

        private string regionCodeField;

        private string countryCodeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public decimal cityId
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
        public string terminalCode
        {
            get
            {
                return this.terminalCodeField;
            }
            set
            {
                this.terminalCodeField = value;
                this.RaisePropertyChanged("terminalCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public decimal index
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool indexSpecified
        {
            get
            {
                return this.indexFieldSpecified;
            }
            set
            {
                this.indexFieldSpecified = value;
                this.RaisePropertyChanged("indexSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string regionCode
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class dpdPossibleESResult : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string resultCodeField;

        private string resultMessageField;

        private possibleExtraService[] extraServiceField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string resultCode
        {
            get
            {
                return this.resultCodeField;
            }
            set
            {
                this.resultCodeField = value;
                this.RaisePropertyChanged("resultCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string resultMessage
        {
            get
            {
                return this.resultMessageField;
            }
            set
            {
                this.resultMessageField = value;
                this.RaisePropertyChanged("resultMessage");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("extraService", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public possibleExtraService[] extraService
        {
            get
            {
                return this.extraServiceField;
            }
            set
            {
                this.extraServiceField = value;
                this.RaisePropertyChanged("extraService");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class possibleExtraService : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string codeField;

        private string nameField;

        private bool isPaidField;

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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
                this.RaisePropertyChanged("name");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public bool isPaid
        {
            get
            {
                return this.isPaidField;
            }
            set
            {
                this.isPaidField = value;
                this.RaisePropertyChanged("isPaid");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getPossibleExtraService", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getPossibleExtraServiceRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public dpdPossibleESRequest request;

        public getPossibleExtraServiceRequest()
        {
        }

        public getPossibleExtraServiceRequest(dpdPossibleESRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getPossibleExtraServiceResponse", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getPossibleExtraServiceResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public dpdPossibleESResult @return;

        public getPossibleExtraServiceResponse()
        {
        }

        public getPossibleExtraServiceResponse(dpdPossibleESResult @return)
        {
            this.@return = @return;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class dpdCitiesCashPayRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private string countryCodeField;

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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class city : object, System.ComponentModel.INotifyPropertyChanged
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

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getCitiesCashPay", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getCitiesCashPayRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public dpdCitiesCashPayRequest request;

        public getCitiesCashPayRequest()
        {
        }

        public getCitiesCashPayRequest(dpdCitiesCashPayRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getCitiesCashPayResponse", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getCitiesCashPayResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("return", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public city[] @return;

        public getCitiesCashPayResponse()
        {
        }

        public getCitiesCashPayResponse(city[] @return)
        {
            this.@return = @return;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class dpdParcelShopRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private string countryCodeField;

        private string regionCodeField;

        private string cityCodeField;

        private string cityNameField;

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
        public string regionCode
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class parcelShop : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string codeField;

        private string parcelShopTypeField;

        private string stateField;

        private address addressField;

        private string brandField;

        private string metroField;

        private string clientDepartmentNumField;

        private geoCoordinates geoCoordinatesField;

        private limits limitsField;

        private schedule[] scheduleField;

        private extraService[] extraServiceField;

        private string[] servicesField;

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
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public string parcelShopType
        {
            get
            {
                return this.parcelShopTypeField;
            }
            set
            {
                this.parcelShopTypeField = value;
                this.RaisePropertyChanged("parcelShopType");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
                this.RaisePropertyChanged("state");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public address address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
                this.RaisePropertyChanged("address");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public string brand
        {
            get
            {
                return this.brandField;
            }
            set
            {
                this.brandField = value;
                this.RaisePropertyChanged("brand");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public string metro
        {
            get
            {
                return this.metroField;
            }
            set
            {
                this.metroField = value;
                this.RaisePropertyChanged("metro");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 6)]
        public string clientDepartmentNum
        {
            get
            {
                return this.clientDepartmentNumField;
            }
            set
            {
                this.clientDepartmentNumField = value;
                this.RaisePropertyChanged("clientDepartmentNum");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 7)]
        public geoCoordinates geoCoordinates
        {
            get
            {
                return this.geoCoordinatesField;
            }
            set
            {
                this.geoCoordinatesField = value;
                this.RaisePropertyChanged("geoCoordinates");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 8)]
        public limits limits
        {
            get
            {
                return this.limitsField;
            }
            set
            {
                this.limitsField = value;
                this.RaisePropertyChanged("limits");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("schedule", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 9)]
        public schedule[] schedule
        {
            get
            {
                return this.scheduleField;
            }
            set
            {
                this.scheduleField = value;
                this.RaisePropertyChanged("schedule");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("extraService", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable = true, Order = 10)]
        public extraService[] extraService
        {
            get
            {
                return this.extraServiceField;
            }
            set
            {
                this.extraServiceField = value;
                this.RaisePropertyChanged("extraService");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 11)]
        [System.Xml.Serialization.XmlArrayItemAttribute("serviceCode", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] services
        {
            get
            {
                return this.servicesField;
            }
            set
            {
                this.servicesField = value;
                this.RaisePropertyChanged("services");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class limits : object, System.ComponentModel.INotifyPropertyChanged
    {

        private decimal maxShipmentWeightField;

        private bool maxShipmentWeightFieldSpecified;

        private decimal maxWeightField;

        private bool maxWeightFieldSpecified;

        private decimal maxLengthField;

        private bool maxLengthFieldSpecified;

        private decimal maxWidthField;

        private bool maxWidthFieldSpecified;

        private decimal maxHeightField;

        private bool maxHeightFieldSpecified;

        private decimal dimensionSumField;

        private bool dimensionSumFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public decimal maxShipmentWeight
        {
            get
            {
                return this.maxShipmentWeightField;
            }
            set
            {
                this.maxShipmentWeightField = value;
                this.RaisePropertyChanged("maxShipmentWeight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxShipmentWeightSpecified
        {
            get
            {
                return this.maxShipmentWeightFieldSpecified;
            }
            set
            {
                this.maxShipmentWeightFieldSpecified = value;
                this.RaisePropertyChanged("maxShipmentWeightSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public decimal maxWeight
        {
            get
            {
                return this.maxWeightField;
            }
            set
            {
                this.maxWeightField = value;
                this.RaisePropertyChanged("maxWeight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxWeightSpecified
        {
            get
            {
                return this.maxWeightFieldSpecified;
            }
            set
            {
                this.maxWeightFieldSpecified = value;
                this.RaisePropertyChanged("maxWeightSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
        public decimal maxLength
        {
            get
            {
                return this.maxLengthField;
            }
            set
            {
                this.maxLengthField = value;
                this.RaisePropertyChanged("maxLength");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxLengthSpecified
        {
            get
            {
                return this.maxLengthFieldSpecified;
            }
            set
            {
                this.maxLengthFieldSpecified = value;
                this.RaisePropertyChanged("maxLengthSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 3)]
        public decimal maxWidth
        {
            get
            {
                return this.maxWidthField;
            }
            set
            {
                this.maxWidthField = value;
                this.RaisePropertyChanged("maxWidth");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxWidthSpecified
        {
            get
            {
                return this.maxWidthFieldSpecified;
            }
            set
            {
                this.maxWidthFieldSpecified = value;
                this.RaisePropertyChanged("maxWidthSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 4)]
        public decimal maxHeight
        {
            get
            {
                return this.maxHeightField;
            }
            set
            {
                this.maxHeightField = value;
                this.RaisePropertyChanged("maxHeight");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool maxHeightSpecified
        {
            get
            {
                return this.maxHeightFieldSpecified;
            }
            set
            {
                this.maxHeightFieldSpecified = value;
                this.RaisePropertyChanged("maxHeightSpecified");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 5)]
        public decimal dimensionSum
        {
            get
            {
                return this.dimensionSumField;
            }
            set
            {
                this.dimensionSumField = value;
                this.RaisePropertyChanged("dimensionSum");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool dimensionSumSpecified
        {
            get
            {
                return this.dimensionSumFieldSpecified;
            }
            set
            {
                this.dimensionSumFieldSpecified = value;
                this.RaisePropertyChanged("dimensionSumSpecified");
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
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getParcelShops", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getParcelShopsRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public dpdParcelShopRequest request;

        public getParcelShopsRequest()
        {
        }

        public getParcelShopsRequest(dpdParcelShopRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getParcelShopsResponse", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getParcelShopsResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public parcelShop[] @return;

        public getParcelShopsResponse()
        {
        }

        public getParcelShopsResponse(parcelShop[] @return)
        {
            this.@return = @return;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.0.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class storagePeriodRequest : object, System.ComponentModel.INotifyPropertyChanged
    {

        private auth authField;

        private string terminalCodeField;

        private string serviceCodeField;

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
        public string terminalCode
        {
            get
            {
                return this.terminalCodeField;
            }
            set
            {
                this.terminalCodeField = value;
                this.RaisePropertyChanged("terminalCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 2)]
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class terminalStoragePeriods : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string terminalCodeField;

        private storagePeriod[] servicesField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 0)]
        public string terminalCode
        {
            get
            {
                return this.terminalCodeField;
            }
            set
            {
                this.terminalCodeField = value;
                this.RaisePropertyChanged("terminalCode");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("services", Form = System.Xml.Schema.XmlSchemaForm.Unqualified, Order = 1)]
        public storagePeriod[] services
        {
            get
            {
                return this.servicesField;
            }
            set
            {
                this.servicesField = value;
                this.RaisePropertyChanged("services");
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20")]
    public partial class storagePeriod : object, System.ComponentModel.INotifyPropertyChanged
    {

        private string serviceCodeField;

        private int daysField;

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
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getStoragePeriod", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getStoragePeriodRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public storagePeriodRequest request;

        public getStoragePeriodRequest()
        {
        }

        public getStoragePeriodRequest(storagePeriodRequest request)
        {
            this.request = request;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName = "getStoragePeriodResponse", WrapperNamespace = "http://dpd.ru/ws/geography/2015-05-20", IsWrapped = true)]
    public partial class getStoragePeriodResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://dpd.ru/ws/geography/2015-05-20", Order = 0)]
        [System.Xml.Serialization.XmlArrayAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("terminal", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public terminalStoragePeriods[] @return;

        public getStoragePeriodResponse()
        {
        }

        public getStoragePeriodResponse(terminalStoragePeriods[] @return)
        {
            this.@return = @return;
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface DPDGeography2Channel : DPDGeography2, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class DPDGeography2Client : System.ServiceModel.ClientBase<DPDGeography2>, DPDGeography2
    {

        public DPDGeography2Client()
        {
        }

        public DPDGeography2Client(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public DPDGeography2Client(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public DPDGeography2Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public DPDGeography2Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getTerminalsSelfDelivery2Response DPDGeography2.getTerminalsSelfDelivery2(getTerminalsSelfDelivery2Request request)
        {
            return base.Channel.getTerminalsSelfDelivery2(request);
        }

        public terminalSelf[] getTerminalsSelfDelivery2(auth auth)
        {
            getTerminalsSelfDelivery2Request inValue = new getTerminalsSelfDelivery2Request();
            inValue.auth = auth;
            getTerminalsSelfDelivery2Response retVal = ((DPDGeography2)(this)).getTerminalsSelfDelivery2(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getTerminalsSelfDelivery2Response> DPDGeography2.getTerminalsSelfDelivery2Async(getTerminalsSelfDelivery2Request request)
        {
            return base.Channel.getTerminalsSelfDelivery2Async(request);
        }

        public System.Threading.Tasks.Task<getTerminalsSelfDelivery2Response> getTerminalsSelfDelivery2Async(auth auth)
        {
            getTerminalsSelfDelivery2Request inValue = new getTerminalsSelfDelivery2Request();
            inValue.auth = auth;
            return ((DPDGeography2)(this)).getTerminalsSelfDelivery2Async(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getPossibleExtraServiceResponse DPDGeography2.getPossibleExtraService(getPossibleExtraServiceRequest request)
        {
            return base.Channel.getPossibleExtraService(request);
        }

        public dpdPossibleESResult getPossibleExtraService(dpdPossibleESRequest request)
        {
            getPossibleExtraServiceRequest inValue = new getPossibleExtraServiceRequest();
            inValue.request = request;
            getPossibleExtraServiceResponse retVal = ((DPDGeography2)(this)).getPossibleExtraService(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getPossibleExtraServiceResponse> DPDGeography2.getPossibleExtraServiceAsync(getPossibleExtraServiceRequest request)
        {
            return base.Channel.getPossibleExtraServiceAsync(request);
        }

        public System.Threading.Tasks.Task<getPossibleExtraServiceResponse> getPossibleExtraServiceAsync(dpdPossibleESRequest request)
        {
            getPossibleExtraServiceRequest inValue = new getPossibleExtraServiceRequest();
            inValue.request = request;
            return ((DPDGeography2)(this)).getPossibleExtraServiceAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getCitiesCashPayResponse DPDGeography2.getCitiesCashPay(getCitiesCashPayRequest request)
        {
            return base.Channel.getCitiesCashPay(request);
        }

        public city[] getCitiesCashPay(dpdCitiesCashPayRequest request)
        {
            getCitiesCashPayRequest inValue = new getCitiesCashPayRequest();
            inValue.request = request;
            getCitiesCashPayResponse retVal = ((DPDGeography2)(this)).getCitiesCashPay(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getCitiesCashPayResponse> DPDGeography2.getCitiesCashPayAsync(getCitiesCashPayRequest request)
        {
            return base.Channel.getCitiesCashPayAsync(request);
        }

        public System.Threading.Tasks.Task<getCitiesCashPayResponse> getCitiesCashPayAsync(dpdCitiesCashPayRequest request)
        {
            getCitiesCashPayRequest inValue = new getCitiesCashPayRequest();
            inValue.request = request;
            return ((DPDGeography2)(this)).getCitiesCashPayAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getParcelShopsResponse DPDGeography2.getParcelShops(getParcelShopsRequest request)
        {
            return base.Channel.getParcelShops(request);
        }

        public parcelShop[] getParcelShops(dpdParcelShopRequest request)
        {
            getParcelShopsRequest inValue = new getParcelShopsRequest();
            inValue.request = request;
            getParcelShopsResponse retVal = ((DPDGeography2)(this)).getParcelShops(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getParcelShopsResponse> DPDGeography2.getParcelShopsAsync(getParcelShopsRequest request)
        {
            return base.Channel.getParcelShopsAsync(request);
        }

        public System.Threading.Tasks.Task<getParcelShopsResponse> getParcelShopsAsync(dpdParcelShopRequest request)
        {
            getParcelShopsRequest inValue = new getParcelShopsRequest();
            inValue.request = request;
            return ((DPDGeography2)(this)).getParcelShopsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        getStoragePeriodResponse DPDGeography2.getStoragePeriod(getStoragePeriodRequest request)
        {
            return base.Channel.getStoragePeriod(request);
        }

        public terminalStoragePeriods[] getStoragePeriod(storagePeriodRequest request)
        {
            getStoragePeriodRequest inValue = new getStoragePeriodRequest();
            inValue.request = request;
            getStoragePeriodResponse retVal = ((DPDGeography2)(this)).getStoragePeriod(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<getStoragePeriodResponse> DPDGeography2.getStoragePeriodAsync(getStoragePeriodRequest request)
        {
            return base.Channel.getStoragePeriodAsync(request);
        }

        public System.Threading.Tasks.Task<getStoragePeriodResponse> getStoragePeriodAsync(storagePeriodRequest request)
        {
            getStoragePeriodRequest inValue = new getStoragePeriodRequest();
            inValue.request = request;
            return ((DPDGeography2)(this)).getStoragePeriodAsync(inValue);
        }
    }
}
