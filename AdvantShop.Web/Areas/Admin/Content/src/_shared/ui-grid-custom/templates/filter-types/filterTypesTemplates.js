import date from './date.html';
import datetime from './datetime.html';
import time from './time.html';
import input from './input.html';
import number from './number.html';
import range from './range.html';
import select from './select.html';
import selectMultiple from './selectMultiple.html';
import phone from './phone.html';

const templates = new Map();
templates.set(`date`, date);
templates.set(`datetime`, datetime);
templates.set(`time`, time);
templates.set(`input`, input);
templates.set(`number`, number);
templates.set(`range`, range);
templates.set(`select`, select);
templates.set(`selectMultiple`, selectMultiple);
templates.set(`phone`, phone);

export default templates;
