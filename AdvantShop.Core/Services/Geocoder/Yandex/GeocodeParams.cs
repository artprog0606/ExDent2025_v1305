using System.Globalization;
using AdvantShop.Core.Common;

namespace AdvantShop.Geocoder.Yandex
{
    public class GeocodeParams
    {
        /// <summary>
        /// <para>Адрес либо географические координаты искомого объекта. Указанные данные определяют тип геокодирования:</para>
        /// <para>Если указан адрес, то он преобразуется в координаты объекта. Этот процесс называется прямым геокодированием.</para>
        /// <para>Если указаны координаты, они преобразуются в адрес объекта. Этот процесс называется обратным геокодированием.</para>
        /// <para>Доступны несколько <a href="https://yandex.ru/dev/maps/geocoder/doc/desc/concepts/input_params.html#input_params__geocode-format">форматов</a> записи координат.</para>
        /// </summary>
        public string Geocode { get; set; }

        /// <summary>
        /// <para>Только если в параметре <see cref="Geocode">Geocode</see> указаны координаты. Порядок записи координат.</para>
        /// <para>Возможные значения:</para>
        /// <list type="bullet">
        ///    <item>
        ///        <term>longlat — долгота, широта</term>
        ///    </item>
        ///    <item>
        ///        <term>latlong — широта, долгота</term>
        ///    </item>
        ///</list>
        /// </summary>
        public Sco Sco { get; set; }

        /// <summary>
        /// <para>Только если в параметре <see cref="Geocode">Geocode</see> указаны координаты. Вид необходимого топонима.</para>
        /// <para>Список допустимых значений:</para>
        /// <list type="bullet">
        ///    <item>
        ///        <term>house — дом</term>
        ///    </item>
        ///    <item>
        ///        <term>street — улица</term>
        ///    </item>
        ///    <item>
        ///        <term>metro — станция метро</term>
        ///    </item>
        ///    <item>
        ///        <term>district — район города</term>
        ///    </item>
        ///    <item>
        ///        <term>locality — населенный пункт (город/поселок/деревня/село/...)</term>
        ///    </item>
        ///</list>
        /// </summary>
        public GeocodeParamsKind Kind { get; set; }

        /// <summary>
        /// <para>Флаг, задающий ограничение поиска указанной областью. Область задается параметрами <see cref="Ll">Ll</see> и <see cref="Spn">Spn</see> либо <see cref="Bbox">Bbox</see>.</para>
        /// <para>Возможные значения:</para>
        /// <list type="bullet">
        ///    <item>
        ///        <term>DontLimit — не ограничивать поиск</term>
        ///    </item>
        ///    <item>
        ///        <term>Limit — ограничивать поиск</term>
        ///    </item>
        ///</list>
        /// </summary>
        public Rspn Rspn { get; set; }

        /// <summary>
        /// Долгота и широта центра области поиска. Протяженность области поиска задается параметром <see cref="Spn">Spn</see>.
        /// </summary>
        public DoubleNumber Ll { get; set; }

        /// <summary>
        /// <para>Протяженность области поиска. Центр области задается параметром <see cref="Ll">Ll</see></para>
        /// <para>Задается двумя числами:</para>
        /// <list type="bullet">
        ///    <item>
        ///        <term>первое обозначает разницу между максимальной и минимальной долготой области</term>
        ///    </item>
        ///    <item>
        ///        <term>второе обозначает разницу между максимальной и минимальной широтой области</term>
        ///    </item>
        ///</list>
        /// </summary>
        /// <remarks>Если в параметре <see cref="Geocode">Geocode</see> указаны координаты и параметр <see cref="GeocodeParamsKind">Kind</see> имеет значение district, параметр <see cref="Spn">Spn</see> не учитывается.</remarks>
        public DoubleNumber Spn { get; set; }

        /// <summary>
        /// Альтернативный способ задания области поиска. Границы задаются в виде географических координат (в последовательности «долгота, широта») левого нижнего и правого верхнего углов области.
        /// </summary>
        /// <remarks>При одновременном использовании параметров <see cref="Bbox">Bbox</see> и <see cref="Ll">Ll</see>+<see cref="Spn">Spn</see>, параметр <see cref="Bbox">Bbox</see> будет более приоритетным.</remarks>
        public Bbox Bbox { get; set; }

        /// <summary>
        /// <para>Максимальное количество возвращаемых объектов. Если указан параметр <see cref="Skip">Skip</see> то значение нужно задать явно.<br />
        /// Значение по умолчанию: 10.<br />
        /// Максимальное допустимое значение: 100.</para>
        /// </summary>
        public byte? Results { get; set; }
        
        /// <summary>
        /// <para>Количество пропускаемых объектов в ответе, начиная с первого. Если указано, нужно также задать значение <see cref="Results">Results</see>. Значение <see cref="Skip">Skip</see> должно нацело делиться на значение <see cref="Results">Results</see>.<br />
        /// Значение по умолчанию: 0.</para>
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// <para>Язык ответа и региональные особенности карты.</para>
        /// <para>Формат записи language_region, где</para>
        /// <list type="bullet">
        ///    <item>
        ///        <term>language — двузначный код языка. Указывается в формате ISO 639-1. Задает язык, на котором будут отображаться названия географических объектов.</term>
        ///    </item>
        ///    <item>
        ///        <term>region — двузначный код страны. Указывается в формате ISO 3166-1. Определяет региональные особенности.</term>
        ///    </item>
        ///</list>
        /// <para>Список поддерживаемых значений:</para>
        /// <list type="bullet">
        ///    <item>
        ///        <term>ru_RU — русский</term>
        ///    </item>
        ///    <item>
        ///        <term>uk_UA — украинский</term>
        ///    </item>
        ///    <item>
        ///        <term>be_BY — белорусский</term>
        ///    </item>
        ///    <item>
        ///        <term>en_RU — ответ на английском, российские особенности карты</term>
        ///    </item>
        ///    <item>
        ///        <term>en_US — ответ на английском, американские особенности карты</term>
        ///    </item>
        ///    <item>
        ///        <term>tr_TR — турецкий (только для карты Турции)</term>
        ///    </item>
        ///</list>
        /// </summary>
        public Lang Lang { get; set; }
    }

    public class Sco : StringEnum<Sco>
    {
        public Sco(string value) : base(value) { }
        
        /// <summary>
        /// Долгота, широта
        /// </summary>
        public static Sco Longlat => new Sco("longlat");

        /// <summary>
        /// Широта, долгота
        /// </summary>
        public static Sco Latlong => new Sco("latlong");
    }

    public class GeocodeParamsKind : StringEnum<GeocodeParamsKind>
    {
        public GeocodeParamsKind(string value) : base(value) { }
            
        /// <summary>
        /// Дом
        /// </summary>
        public static GeocodeParamsKind House => new GeocodeParamsKind("house");
            
        /// <summary>
        /// Улица
        /// </summary>
        public static GeocodeParamsKind Street => new GeocodeParamsKind("street");
            
        /// <summary>
        /// Станция метро
        /// </summary>
        public static GeocodeParamsKind Metro => new GeocodeParamsKind("metro");
            
        /// <summary>
        /// Район города
        /// </summary>
        public static GeocodeParamsKind District => new GeocodeParamsKind("district");
            
        /// <summary>
        /// Населенный пункт (город/поселок/деревня/село/...)
        /// </summary>
        public static GeocodeParamsKind Locality => new GeocodeParamsKind("locality");
    }

    public class Rspn : IntegerEnum<Rspn>
    {
        public Rspn(int value) : base(value) { }
            
        /// <summary>
        /// Не ограничивать поиск
        /// </summary>
        public static Rspn DontLimit => new Rspn(0);
            
        /// <summary>
        /// Ограничивать поиск
        /// </summary>
        public static Rspn Limit => new Rspn(1);
    }

    public class DoubleNumber
    {
        public decimal First { get; set; }
        public decimal Second { get; set; }

        public override string ToString() =>
            $"{First.ToString(CultureInfo.InvariantCulture)},{Second.ToString(CultureInfo.InvariantCulture)}";
    }

    public class Bbox
    {
        public DoubleNumber BottomLeft { get; set; }
        public DoubleNumber TopRight { get; set; }

        public override string ToString() => $"{BottomLeft}~{TopRight}";
    }

    public class Lang : StringEnum<Lang>
    {
        public Lang(string value) : base(value) { }
        
        /// <summary>
        /// Русский
        /// </summary>
        public static Lang Russian => new Lang("ru_RU");
        
        /// <summary>
        /// Украинский
        /// </summary>
        public static Lang Ukrainian => new Lang("uk_UA");
        
        /// <summary>
        /// Белорусский
        /// </summary>
        public static Lang Belarusian => new Lang("be_BY");
        
        /// <summary>
        /// Турецкий
        /// </summary>
        public static Lang Turkish => new Lang("tr_TR");
        
        /// <summary>
        /// ответ на английском, российские особенности карты
        /// </summary>
        public static Lang EnRu => new Lang("en_RU");
        
        /// <summary>
        /// ответ на английском, американские особенности карты
        /// </summary>
        public static Lang American => new Lang("en_US");
   
    }
}