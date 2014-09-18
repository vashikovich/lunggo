﻿goog.i18n.ordinalRules = {}; goog.i18n.ordinalRules.Keyword = { ZERO: "zero", ONE: "one", TWO: "two", FEW: "few", MANY: "many", OTHER: "other" }; goog.i18n.ordinalRules.defaultSelect_ = function (a, b) { return goog.i18n.ordinalRules.Keyword.OTHER }; goog.i18n.ordinalRules.decimals_ = function (a) { a += ""; var b = a.indexOf("."); return -1 == b ? 0 : a.length - b - 1 }; goog.i18n.ordinalRules.get_vf_ = function (a, b) { var c = void 0 === b ? Math.min(goog.i18n.ordinalRules.decimals_(a), 3) : b, d = Math.pow(10, c); return { v: c, f: (a * d | 0) % d } };
goog.i18n.ordinalRules.get_wt_ = function (a, b) { if (0 === b) return { w: 0, t: 0 }; for (; 0 === b % 10;) b /= 10, a--; return { w: a, t: b } }; goog.i18n.ordinalRules.enSelect_ = function (a, b) { return 1 == a % 10 && 11 != a % 100 ? goog.i18n.ordinalRules.Keyword.ONE : 2 == a % 10 && 12 != a % 100 ? goog.i18n.ordinalRules.Keyword.TWO : 3 == a % 10 && 13 != a % 100 ? goog.i18n.ordinalRules.Keyword.FEW : goog.i18n.ordinalRules.Keyword.OTHER }; goog.i18n.ordinalRules.select = goog.i18n.ordinalRules.defaultSelect_;
goog.isDef(window.locale) && goog.isDef(window.lang) && goog.isDef(window.country) && "en" == window.lang && (goog.i18n.ordinalRules.select = goog.i18n.ordinalRules.enSelect_); goog.i18n.DateTimeParse = function (a) { this.patternParts_ = []; "number" == typeof a ? this.applyStandardPattern_(a) : this.applyPattern_(a) }; goog.i18n.DateTimeParse.ambiguousYearCenturyStart = 80;
goog.i18n.DateTimeParse.prototype.applyPattern_ = function (a) {
    for (var b = !1, c = "", d = 0; d < a.length; d++) {
        var e = a.charAt(d); if (" " == e) for (0 < c.length && (this.patternParts_.push({ text: c, count: 0, abutStart: !1 }), c = ""), this.patternParts_.push({ text: " ", count: 0, abutStart: !1 }) ; d < a.length - 1 && " " == a.charAt(d + 1) ;) d++; else if (b) "'" == e ? d + 1 < a.length && "'" == a.charAt(d + 1) ? (c += "'", d++) : b = !1 : c += e; else if (0 <= goog.i18n.DateTimeParse.PATTERN_CHARS_.indexOf(e)) {
            0 < c.length && (this.patternParts_.push({ text: c, count: 0, abutStart: !1 }),
            c = ""); var f = this.getNextCharCount_(a, d); this.patternParts_.push({ text: e, count: f, abutStart: !1 }); d += f - 1
        } else "'" == e ? d + 1 < a.length && "'" == a.charAt(d + 1) ? (c += "'", d++) : b = !0 : c += e
    } 0 < c.length && this.patternParts_.push({ text: c, count: 0, abutStart: !1 }); this.markAbutStart_()
};
goog.i18n.DateTimeParse.prototype.applyStandardPattern_ = function (a) { var b; a > goog.i18n.DateTimeFormat.Format.SHORT_DATETIME && (a = goog.i18n.DateTimeFormat.Format.MEDIUM_DATETIME); 4 > a ? b = goog.i18n.DateTimeSymbols.DATEFORMATS[a] : 8 > a ? b = goog.i18n.DateTimeSymbols.TIMEFORMATS[a - 4] : (b = goog.i18n.DateTimeSymbols.DATETIMEFORMATS[a - 8], b = b.replace("{1}", goog.i18n.DateTimeSymbols.DATEFORMATS[a - 8]), b = b.replace("{0}", goog.i18n.DateTimeSymbols.TIMEFORMATS[a - 8])); this.applyPattern_(b) };
goog.i18n.DateTimeParse.prototype.parse = function (a, b, c) { return this.internalParse_(a, b, c || 0, !1) }; goog.i18n.DateTimeParse.prototype.strictParse = function (a, b, c) { return this.internalParse_(a, b, c || 0, !0) };
goog.i18n.DateTimeParse.prototype.internalParse_ = function (a, b, c, d) {
    for (var e = new goog.i18n.DateTimeParse.MyDate_, f = [c], g = -1, h = 0, k = 0, l = 0; l < this.patternParts_.length; l++) if (0 < this.patternParts_[l].count) if (0 > g && this.patternParts_[l].abutStart && (g = l, h = c, k = 0), 0 <= g) { var m = this.patternParts_[l].count; if (l == g && (m -= k, k++, 0 == m)) return 0; this.subParse_(a, f, this.patternParts_[l], m, e) || (l = g - 1, f[0] = h) } else { if (g = -1, !this.subParse_(a, f, this.patternParts_[l], 0, e)) return 0 } else {
        g = -1; if (" " == this.patternParts_[l].text.charAt(0)) {
            if (m =
            f[0], this.skipSpace_(a, f), f[0] > m) continue
        } else if (a.indexOf(this.patternParts_[l].text, f[0]) == f[0]) { f[0] += this.patternParts_[l].text.length; continue } return 0
    } return e.calcDate_(b, d) ? f[0] - c : 0
}; goog.i18n.DateTimeParse.prototype.getNextCharCount_ = function (a, b) { for (var c = a.charAt(b), d = b + 1; d < a.length && a.charAt(d) == c;) d++; return d - b }; goog.i18n.DateTimeParse.PATTERN_CHARS_ = "GyMdkHmsSEDahKzZvQL"; goog.i18n.DateTimeParse.NUMERIC_FORMAT_CHARS_ = "MydhHmsSDkK";
goog.i18n.DateTimeParse.prototype.isNumericField_ = function (a) { if (0 >= a.count) return !1; var b = goog.i18n.DateTimeParse.NUMERIC_FORMAT_CHARS_.indexOf(a.text.charAt(0)); return 0 < b || 0 == b && 3 > a.count }; goog.i18n.DateTimeParse.prototype.markAbutStart_ = function () { for (var a = !1, b = 0; b < this.patternParts_.length; b++) this.isNumericField_(this.patternParts_[b]) ? !a && b + 1 < this.patternParts_.length && this.isNumericField_(this.patternParts_[b + 1]) && (a = !0, this.patternParts_[b].abutStart = !0) : a = !1 };
goog.i18n.DateTimeParse.prototype.skipSpace_ = function (a, b) { var c = a.substring(b[0]).match(/^\s+/); c && (b[0] += c[0].length) };
goog.i18n.DateTimeParse.prototype.subParse_ = function (a, b, c, d, e) {
    this.skipSpace_(a, b); var f = b[0], g = c.text.charAt(0), h = -1; if (this.isNumericField_(c)) if (0 < d) { if (f + d > a.length) return !1; h = this.parseInt_(a.substring(0, f + d), b) } else h = this.parseInt_(a, b); switch (g) {
        case "G": return e.era = this.matchString_(a, b, goog.i18n.DateTimeSymbols.ERAS), !0; case "M": case "L": return this.subParseMonth_(a, b, e, h); case "E": return this.subParseDayOfWeek_(a, b, e); case "a": return e.ampm = this.matchString_(a, b, goog.i18n.DateTimeSymbols.AMPMS),
        !0; case "y": return this.subParseYear_(a, b, f, h, c, e); case "Q": return this.subParseQuarter_(a, b, e, h); case "d": return e.day = h, !0; case "S": return this.subParseFractionalSeconds_(h, b, f, e); case "h": 12 == h && (h = 0); case "K": case "H": case "k": return e.hours = h, !0; case "m": return e.minutes = h, !0; case "s": return e.seconds = h, !0; case "z": case "Z": case "v": return this.subparseTimeZoneInGMT_(a, b, e); default: return !1
    }
};
goog.i18n.DateTimeParse.prototype.subParseYear_ = function (a, b, c, d, e, f) { var g; if (0 > d) { g = a.charAt(b[0]); if ("+" != g && "-" != g) return !1; b[0]++; d = this.parseInt_(a, b); if (0 > d) return !1; "-" == g && (d = -d) } g || 2 != b[0] - c || 2 != e.count ? f.year = d : f.setTwoDigitYear_(d); return !0 };
goog.i18n.DateTimeParse.prototype.subParseMonth_ = function (a, b, c, d) { if (0 > d) { d = goog.i18n.DateTimeSymbols.MONTHS.concat(goog.i18n.DateTimeSymbols.STANDALONEMONTHS).concat(goog.i18n.DateTimeSymbols.SHORTMONTHS).concat(goog.i18n.DateTimeSymbols.STANDALONESHORTMONTHS); d = this.matchString_(a, b, d); if (0 > d) return !1; c.month = d % 12 } else c.month = d - 1; return !0 };
goog.i18n.DateTimeParse.prototype.subParseQuarter_ = function (a, b, c, d) { if (0 > d) { d = this.matchString_(a, b, goog.i18n.DateTimeSymbols.QUARTERS); 0 > d && (d = this.matchString_(a, b, goog.i18n.DateTimeSymbols.SHORTQUARTERS)); if (0 > d) return !1; c.month = 3 * d; c.day = 1; return !0 } return !1 }; goog.i18n.DateTimeParse.prototype.subParseDayOfWeek_ = function (a, b, c) { var d = this.matchString_(a, b, goog.i18n.DateTimeSymbols.WEEKDAYS); 0 > d && (d = this.matchString_(a, b, goog.i18n.DateTimeSymbols.SHORTWEEKDAYS)); if (0 > d) return !1; c.dayOfWeek = d; return !0 };
goog.i18n.DateTimeParse.prototype.subParseFractionalSeconds_ = function (a, b, c, d) { b = b[0] - c; d.milliseconds = 3 > b ? a * Math.pow(10, 3 - b) : Math.round(a / Math.pow(10, b - 3)); return !0 }; goog.i18n.DateTimeParse.prototype.subparseTimeZoneInGMT_ = function (a, b, c) { a.indexOf("GMT", b[0]) == b[0] && (b[0] += 3); return this.parseTimeZoneOffset_(a, b, c) };
goog.i18n.DateTimeParse.prototype.parseTimeZoneOffset_ = function (a, b, c) { if (b[0] >= a.length) return c.tzOffset = 0, !0; var d = 1; switch (a.charAt(b[0])) { case "-": d = -1; case "+": b[0]++ } var e = b[0], f = this.parseInt_(a, b); if (0 == f && b[0] == e) return !1; var g; if (b[0] < a.length && ":" == a.charAt(b[0])) { g = 60 * f; b[0]++; e = b[0]; f = this.parseInt_(a, b); if (0 == f && b[0] == e) return !1; g += f } else g = f, g = 24 > g && 2 >= b[0] - e ? 60 * g : g % 100 + g / 100 * 60; c.tzOffset = -(g * d); return !0 };
goog.i18n.DateTimeParse.prototype.parseInt_ = function (a, b) { if (goog.i18n.DateTimeSymbols.ZERODIGIT) { for (var c = [], d = b[0]; d < a.length; d++) { var e = a.charCodeAt(d) - goog.i18n.DateTimeSymbols.ZERODIGIT; c.push(0 <= e && 9 >= e ? String.fromCharCode(e + 48) : a.charAt(d)) } a = c.join("") } else a = a.substring(b[0]); c = a.match(/^\d+/); if (!c) return -1; b[0] += c[0].length; return parseInt(c[0], 10) };
goog.i18n.DateTimeParse.prototype.matchString_ = function (a, b, c) { var d = 0, e = -1; a = a.substring(b[0]).toLowerCase(); for (var f = 0; f < c.length; f++) { var g = c[f].length; g > d && 0 == a.indexOf(c[f].toLowerCase()) && (e = f, d = g) } 0 <= e && (b[0] += d); return e }; goog.i18n.DateTimeParse.MyDate_ = function () { };
goog.i18n.DateTimeParse.MyDate_.prototype.setTwoDigitYear_ = function (a) { var b = (new Date).getFullYear() - goog.i18n.DateTimeParse.ambiguousYearCenturyStart, c = b % 100; this.ambiguousYear = a == c; return this.year = a += 100 * Math.floor(b / 100) + (a < c ? 100 : 0) };
goog.i18n.DateTimeParse.MyDate_.prototype.calcDate_ = function (a, b) {
    void 0 != this.era && void 0 != this.year && 0 == this.era && 0 < this.year && (this.year = -(this.year - 1)); void 0 != this.year && a.setFullYear(this.year); var c = a.getDate(); a.setDate(1); void 0 != this.month && a.setMonth(this.month); if (void 0 != this.day) a.setDate(this.day); else { var d = goog.date.getNumberOfDaysInMonth(a.getFullYear(), a.getMonth()); a.setDate(c > d ? d : c) } goog.isFunction(a.setHours) && (void 0 == this.hours && (this.hours = a.getHours()), void 0 != this.ampm &&
    0 < this.ampm && 12 > this.hours && (this.hours += 12), a.setHours(this.hours)); goog.isFunction(a.setMinutes) && void 0 != this.minutes && a.setMinutes(this.minutes); goog.isFunction(a.setSeconds) && void 0 != this.seconds && a.setSeconds(this.seconds); goog.isFunction(a.setMilliseconds) && void 0 != this.milliseconds && a.setMilliseconds(this.milliseconds); if (b && (void 0 != this.year && this.year != a.getFullYear() || void 0 != this.month && this.month != a.getMonth() || void 0 != this.day && this.day != a.getDate() || 24 <= this.hours || 60 <= this.minutes ||
    60 <= this.seconds || 1E3 <= this.milliseconds)) return !1; void 0 != this.tzOffset && (c = a.getTimezoneOffset(), a.setTime(a.getTime() + 6E4 * (this.tzOffset - c))); this.ambiguousYear && (c = new Date, c.setFullYear(c.getFullYear() - goog.i18n.DateTimeParse.ambiguousYearCenturyStart), a.getTime() < c.getTime() && a.setFullYear(c.getFullYear() + 100)); if (void 0 != this.dayOfWeek) if (void 0 == this.day) c = (7 + this.dayOfWeek - a.getDay()) % 7, 3 < c && (c -= 7), d = a.getMonth(), a.setDate(a.getDate() + c), a.getMonth() != d && a.setDate(a.getDate() + (0 < c ? -7 : 7));
    else if (this.dayOfWeek != a.getDay()) return !1; return !0
}; goog.i18n.pluralRules = {}; goog.i18n.pluralRules.Keyword = { ZERO: "zero", ONE: "one", TWO: "two", FEW: "few", MANY: "many", OTHER: "other" }; goog.i18n.pluralRules.defaultSelect_ = function (a, b) { return goog.i18n.pluralRules.Keyword.OTHER }; goog.i18n.pluralRules.decimals_ = function (a) { a += ""; var b = a.indexOf("."); return -1 == b ? 0 : a.length - b - 1 }; goog.i18n.pluralRules.get_vf_ = function (a, b) { var c = void 0 === b ? Math.min(goog.i18n.pluralRules.decimals_(a), 3) : b, d = Math.pow(10, c); return { v: c, f: (a * d | 0) % d } };
goog.i18n.pluralRules.get_wt_ = function (a, b) { if (0 === b) return { w: 0, t: 0 }; for (; 0 === b % 10;) b /= 10, a--; return { w: a, t: b } }; goog.i18n.pluralRules.enSelect_ = function (a, b) { var c = a | 0, d = goog.i18n.pluralRules.get_vf_(a, b); return 1 == c && 0 == d.v ? goog.i18n.pluralRules.Keyword.ONE : goog.i18n.pluralRules.Keyword.OTHER }; goog.i18n.pluralRules.select = goog.i18n.pluralRules.defaultSelect_; goog.isDef(window.locale) && goog.isDef(window.lang) && goog.isDef(window.country) && "en" == window.lang && (goog.i18n.pluralRules.select = goog.i18n.pluralRules.enSelect_); tv.i18n.type = {}; tv.i18n.ContentResource = function (a, b) { this.id = a.id; this.map = a.map; this.format = b }; tv.i18n.ContentResource.prototype.isEmpty = function () { for (var a in this.map) return !1; return !0 }; tv.i18n.ContentResource.prototype.get = function (a, b) { var c = this.map[a]; if (!1 == $.isPlainObject(c)) return a; null == b && (b = {}); var d = c.value; "icu" == c.type && (d = this.format.message(d, b)); "true" == c.escape && (d = goog.string.htmlEscape(d)); return d }; tv.i18n.ContentResource.prototype.has = function (a) { return $.isPlainObject(this.map[a]) }; window.format = new tv.i18n.Format; goog.i18n.MessageFormat = function (a) { this.literals_ = []; this.parsedPattern_ = []; this.numberFormatter_ = new goog.i18n.NumberFormat(goog.i18n.NumberFormat.Format.DECIMAL); this.parsePattern_(a) }; goog.i18n.MessageFormat.LITERAL_PLACEHOLDER_ = "\ufddf_"; goog.i18n.MessageFormat.Element_ = { STRING: 0, BLOCK: 1 }; goog.i18n.MessageFormat.BlockType_ = { PLURAL: 0, ORDINAL: 1, SELECT: 2, SIMPLE: 3, STRING: 4, UNKNOWN: 5 }; goog.i18n.MessageFormat.OTHER_ = "other"; goog.i18n.MessageFormat.REGEX_LITERAL_ = RegExp("'([{}#].*?)'", "g");
goog.i18n.MessageFormat.REGEX_DOUBLE_APOSTROPHE_ = RegExp("''", "g"); goog.i18n.MessageFormat.prototype.format = function (a) { return this.format_(a, !1) }; goog.i18n.MessageFormat.prototype.formatIgnoringPound = function (a) { return this.format_(a, !0) };
goog.i18n.MessageFormat.prototype.format_ = function (a, b) { if (0 == this.parsedPattern_.length) return ""; var c = []; this.formatBlock_(this.parsedPattern_, a, b, c); c = c.join(""); for (b || goog.asserts.assert(-1 == c.search("#"), "Not all # were replaced.") ; 0 < this.literals_.length;) c = c.replace(this.buildPlaceholder_(this.literals_), this.literals_.pop()); return c };
goog.i18n.MessageFormat.prototype.formatBlock_ = function (a, b, c, d) {
    for (var e = 0; e < a.length; e++) switch (a[e].type) {
        case goog.i18n.MessageFormat.BlockType_.STRING: d.push(a[e].value); break; case goog.i18n.MessageFormat.BlockType_.SIMPLE: var f = a[e].value; this.formatSimplePlaceholder_(f, b, d); break; case goog.i18n.MessageFormat.BlockType_.SELECT: f = a[e].value; this.formatSelectBlock_(f, b, c, d); break; case goog.i18n.MessageFormat.BlockType_.PLURAL: f = a[e].value; this.formatPluralOrdinalBlock_(f, b, goog.i18n.pluralRules.select,
        c, d); break; case goog.i18n.MessageFormat.BlockType_.ORDINAL: f = a[e].value; this.formatPluralOrdinalBlock_(f, b, goog.i18n.ordinalRules.select, c, d); break; default: goog.asserts.fail("Unrecognized block type.")
    }
}; goog.i18n.MessageFormat.prototype.formatSimplePlaceholder_ = function (a, b, c) { b = b[a]; goog.isDef(b) ? (this.literals_.push(b), c.push(this.buildPlaceholder_(this.literals_))) : c.push("Undefined parameter - " + a) };
goog.i18n.MessageFormat.prototype.formatSelectBlock_ = function (a, b, c, d) { var e = a.argumentIndex; goog.isDef(b[e]) ? (e = a[b[e]], goog.isDef(e) || (e = a[goog.i18n.MessageFormat.OTHER_], goog.asserts.assertArray(e, "Invalid option or missing other option for select block.")), this.formatBlock_(e, b, c, d)) : d.push("Undefined parameter - " + e) };
goog.i18n.MessageFormat.prototype.formatPluralOrdinalBlock_ = function (a, b, c, d, e) {
    var f = a.argumentIndex, g = a.argumentOffset, h = +b[f]; isNaN(h) ? e.push("Undefined or invalid parameter - " + f) : (g = h - g, f = a[b[f]], goog.isDef(f) || (goog.asserts.assert(0 <= g, "Argument index smaller than offset."), c = this.numberFormatter_.getMinimumFractionDigits ? c(g, this.numberFormatter_.getMinimumFractionDigits()) : c(g), goog.asserts.assertString(c, "Invalid plural key."), f = a[c], goog.isDef(f) || (f = a[goog.i18n.MessageFormat.OTHER_]), goog.asserts.assertArray(f,
    "Invalid option or missing other option for plural block.")), a = [], this.formatBlock_(f, b, d, a), b = a.join(""), goog.asserts.assertString(b, "Empty block in plural."), d ? e.push(b) : (d = this.numberFormatter_.format(g), e.push(b.replace(/#/g, d))))
}; goog.i18n.MessageFormat.prototype.parsePattern_ = function (a) { a && (a = this.insertPlaceholders_(a), this.parsedPattern_ = this.parseBlock_(a)) };
goog.i18n.MessageFormat.prototype.insertPlaceholders_ = function (a) { var b = this.literals_, c = goog.bind(this.buildPlaceholder_, this); a = a.replace(goog.i18n.MessageFormat.REGEX_DOUBLE_APOSTROPHE_, function () { b.push("'"); return c(b) }); return a = a.replace(goog.i18n.MessageFormat.REGEX_LITERAL_, function (a, e) { b.push(e); return c(b) }) };
goog.i18n.MessageFormat.prototype.extractParts_ = function (a) {
    var b = 0, c = [], d = [], e = /[{}]/g; e.lastIndex = 0; for (var f; f = e.exec(a) ;) { var g = f.index; "}" == f[0] ? (f = c.pop(), goog.asserts.assert(goog.isDef(f) && "{" == f, "No matching { for }."), 0 == c.length && (f = {}, f.type = goog.i18n.MessageFormat.Element_.BLOCK, f.value = a.substring(b, g), d.push(f), b = g + 1)) : (0 == c.length && (b = a.substring(b, g), "" != b && d.push({ type: goog.i18n.MessageFormat.Element_.STRING, value: b }), b = g + 1), c.push("{")) } goog.asserts.assert(0 == c.length, "There are mismatched { or } in the pattern.");
    b = a.substring(b); "" != b && d.push({ type: goog.i18n.MessageFormat.Element_.STRING, value: b }); return d
}; goog.i18n.MessageFormat.PLURAL_BLOCK_RE_ = /^\s*(\w+)\s*,\s*plural\s*,(?:\s*offset:(\d+))?/; goog.i18n.MessageFormat.ORDINAL_BLOCK_RE_ = /^\s*(\w+)\s*,\s*selectordinal\s*,/; goog.i18n.MessageFormat.SELECT_BLOCK_RE_ = /^\s*(\w+)\s*,\s*select\s*,/;
goog.i18n.MessageFormat.prototype.parseBlockType_ = function (a) { return goog.i18n.MessageFormat.PLURAL_BLOCK_RE_.test(a) ? goog.i18n.MessageFormat.BlockType_.PLURAL : goog.i18n.MessageFormat.ORDINAL_BLOCK_RE_.test(a) ? goog.i18n.MessageFormat.BlockType_.ORDINAL : goog.i18n.MessageFormat.SELECT_BLOCK_RE_.test(a) ? goog.i18n.MessageFormat.BlockType_.SELECT : /^\s*\w+\s*/.test(a) ? goog.i18n.MessageFormat.BlockType_.SIMPLE : goog.i18n.MessageFormat.BlockType_.UNKNOWN };
goog.i18n.MessageFormat.prototype.parseBlock_ = function (a) {
    var b = []; a = this.extractParts_(a); for (var c = 0; c < a.length; c++) {
        var d = {}; if (goog.i18n.MessageFormat.Element_.STRING == a[c].type) d.type = goog.i18n.MessageFormat.BlockType_.STRING, d.value = a[c].value; else if (goog.i18n.MessageFormat.Element_.BLOCK == a[c].type) switch (this.parseBlockType_(a[c].value)) {
            case goog.i18n.MessageFormat.BlockType_.SELECT: d.type = goog.i18n.MessageFormat.BlockType_.SELECT; d.value = this.parseSelectBlock_(a[c].value); break; case goog.i18n.MessageFormat.BlockType_.PLURAL: d.type =
            goog.i18n.MessageFormat.BlockType_.PLURAL; d.value = this.parsePluralBlock_(a[c].value); break; case goog.i18n.MessageFormat.BlockType_.ORDINAL: d.type = goog.i18n.MessageFormat.BlockType_.ORDINAL; d.value = this.parseOrdinalBlock_(a[c].value); break; case goog.i18n.MessageFormat.BlockType_.SIMPLE: d.type = goog.i18n.MessageFormat.BlockType_.SIMPLE; d.value = a[c].value; break; default: goog.asserts.fail("Unknown block type.")
        } else goog.asserts.fail("Unknown part of the pattern."); b.push(d)
    } return b
};
goog.i18n.MessageFormat.prototype.parseSelectBlock_ = function (a) {
    var b = ""; a = a.replace(goog.i18n.MessageFormat.SELECT_BLOCK_RE_, function (a, c) { b = c; return "" }); var c = {}; c.argumentIndex = b; a = this.extractParts_(a); for (var d = 0; d < a.length;) {
        var e = a[d].value; goog.asserts.assertString(e, "Missing select key element."); d++; goog.asserts.assert(d < a.length, "Missing or invalid select value element."); if (goog.i18n.MessageFormat.Element_.BLOCK == a[d].type) var f = this.parseBlock_(a[d].value); else goog.asserts.fail("Expected block type.");
        c[e.replace(/\s/g, "")] = f; d++
    } goog.asserts.assertArray(c[goog.i18n.MessageFormat.OTHER_], "Missing other key in select statement."); return c
};
goog.i18n.MessageFormat.prototype.parsePluralBlock_ = function (a) {
    var b = "", c = 0; a = a.replace(goog.i18n.MessageFormat.PLURAL_BLOCK_RE_, function (a, d, e) { b = d; e && (c = parseInt(e, 10)); return "" }); var d = {}; d.argumentIndex = b; d.argumentOffset = c; a = this.extractParts_(a); for (var e = 0; e < a.length;) {
        var f = a[e].value; goog.asserts.assertString(f, "Missing plural key element."); e++; goog.asserts.assert(e < a.length, "Missing or invalid plural value element."); if (goog.i18n.MessageFormat.Element_.BLOCK == a[e].type) var g = this.parseBlock_(a[e].value);
        else goog.asserts.fail("Expected block type."); d[f.replace(/\s*(?:=)?(\w+)\s*/, "$1")] = g; e++
    } goog.asserts.assertArray(d[goog.i18n.MessageFormat.OTHER_], "Missing other key in plural statement."); return d
};
goog.i18n.MessageFormat.prototype.parseOrdinalBlock_ = function (a) {
    var b = ""; a = a.replace(goog.i18n.MessageFormat.ORDINAL_BLOCK_RE_, function (a, c) { b = c; return "" }); var c = {}; c.argumentIndex = b; c.argumentOffset = 0; a = this.extractParts_(a); for (var d = 0; d < a.length;) {
        var e = a[d].value; goog.asserts.assertString(e, "Missing ordinal key element."); d++; goog.asserts.assert(d < a.length, "Missing or invalid ordinal value element."); if (goog.i18n.MessageFormat.Element_.BLOCK == a[d].type) var f = this.parseBlock_(a[d].value); else goog.asserts.fail("Expected block type.");
        c[e.replace(/\s*(?:=)?(\w+)\s*/, "$1")] = f; d++
    } goog.asserts.assertArray(c[goog.i18n.MessageFormat.OTHER_], "Missing other key in selectordinal statement."); return c
}; goog.i18n.MessageFormat.prototype.buildPlaceholder_ = function (a) { goog.asserts.assert(0 < a.length, "Literal array is empty."); a = (a.length - 1).toString(10); return goog.i18n.MessageFormat.LITERAL_PLACEHOLDER_ + a + "_" };

//@ sourceMappingURL=../../map/i18n.js.map