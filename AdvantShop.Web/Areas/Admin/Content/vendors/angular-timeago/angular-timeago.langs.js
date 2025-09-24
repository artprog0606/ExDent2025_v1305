angular.module('yaru22.angular-timeago').config(['timeAgoSettings', function (timeAgoSettings) {
  timeAgoSettings.strings['ru'] = {
    prefixAgo: null,
    prefixFromNow: null,
    suffixAgo: 'назад',
    suffixFromNow: 'с текущего момента',
    seconds: 'менее минуты',
    minute: 'около минуты',
    minutes: '%d мин.',
    hour: 'около часа',
    hours: 'около %d ч.',
    day: 'день',
    days: '%d дн.',
    month: 'около месяца',
    months: '%d мес.',
    year: 'около года',
    years: '%d года',
    numbers: []
  };
}]);

angular.module('yaru22.angular-timeago').config(["timeAgoSettings", function(timeAgoSettings) {
  timeAgoSettings.strings['en'] = {
    prefixAgo: null,
    prefixFromNow: null,
    suffixAgo: 'ago',
    suffixFromNow: 'from now',
    seconds: 'less than a minute',
    minute: 'about a minute',
    minutes: '%d minutes',
    hour: 'about an hour',
    hours: 'about %d hours',
    day: 'a day',
    days: '%d days',
    month: 'about a month',
    months: '%d months',
    year: 'about a year',
    years: '%d years',
    numbers: []
  };
}]);