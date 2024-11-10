"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
var __spreadArray = (this && this.__spreadArray) || function (to, from, pack) {
    if (pack || arguments.length === 2) for (var i = 0, l = from.length, ar; i < l; i++) {
        if (ar || !(i in from)) {
            if (!ar) ar = Array.prototype.slice.call(from, 0, i);
            ar[i] = from[i];
        }
    }
    return to.concat(ar || Array.prototype.slice.call(from));
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.ChartComponent = void 0;
var core_1 = require("@angular/core");
var rxjs_1 = require("rxjs");
var button_1 = require("@angular/material/button");
var ng_apexcharts_1 = require("ng-apexcharts");
var ChartComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-chart',
            standalone: true,
            imports: [
                ng_apexcharts_1.NgApexchartsModule,
                button_1.MatButton,
            ],
            templateUrl: './chart.component.html',
            styleUrls: ['./chart.component.css']
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var _currencyPair_decorators;
    var _currencyPair_initializers = [];
    var _currencyPair_extraInitializers = [];
    var _buyEvent_decorators;
    var _buyEvent_initializers = [];
    var _buyEvent_extraInitializers = [];
    var ChartComponent = _classThis = /** @class */ (function () {
        function ChartComponent_1(binanceService) {
            this.binanceService = binanceService;
            this.currencyPair = __runInitializers(this, _currencyPair_initializers, void 0);
            this.buyEvent = (__runInitializers(this, _currencyPair_extraInitializers), __runInitializers(this, _buyEvent_initializers, new core_1.EventEmitter()));
            this.trades = (__runInitializers(this, _buyEvent_extraInitializers), []);
            this.priceObservable = new rxjs_1.Subscription;
            this.chartSeries = [
                {
                    name: '',
                    data: []
                }
            ];
            this.chartOptions = {
                chart: {
                    type: 'line',
                    height: 350
                },
                xaxis: {
                    type: 'datetime',
                },
                title: {
                    text: '',
                }
            };
        }
        ChartComponent_1.prototype.ngOnInit = function () {
            this.chartSeries[0].name = "".concat(this.currencyPair, " Price");
            this.chartOptions.title.text = "".concat(this.currencyPair, " Price");
            this.priceObservable = this.getPrice();
        };
        ChartComponent_1.prototype.getPrice = function () {
            var _this = this;
            return this.binanceService.getCryptoPriceUpdates(this.currencyPair).subscribe(function (data) {
                var price = parseFloat(data.c);
                var currentTime = new Date().getTime();
                var trade = {
                    price: price,
                    date: new Date(),
                    currency: _this.currencyPair,
                    number: 1,
                };
                _this.trades.push(trade);
                _this.chartSeries[0].data.push({ x: currentTime, y: price });
                if (_this.chartSeries[0].data.length > 20) {
                    _this.chartSeries[0].data.shift();
                }
                _this.chartSeries = __spreadArray([], _this.chartSeries, true);
            }, function (error) { return console.log("Error occured: ".concat(error)); });
        };
        ChartComponent_1.prototype.buy = function () {
            this.buyEvent.emit(this.trades);
        };
        ChartComponent_1.prototype.ngOnDestroy = function () {
            if (this.priceObservable) {
                this.priceObservable.unsubscribe();
            }
        };
        return ChartComponent_1;
    }());
    __setFunctionName(_classThis, "ChartComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        _currencyPair_decorators = [(0, core_1.Input)()];
        _buyEvent_decorators = [(0, core_1.Output)()];
        __esDecorate(null, null, _currencyPair_decorators, { kind: "field", name: "currencyPair", static: false, private: false, access: { has: function (obj) { return "currencyPair" in obj; }, get: function (obj) { return obj.currencyPair; }, set: function (obj, value) { obj.currencyPair = value; } }, metadata: _metadata }, _currencyPair_initializers, _currencyPair_extraInitializers);
        __esDecorate(null, null, _buyEvent_decorators, { kind: "field", name: "buyEvent", static: false, private: false, access: { has: function (obj) { return "buyEvent" in obj; }, get: function (obj) { return obj.buyEvent; }, set: function (obj, value) { obj.buyEvent = value; } }, metadata: _metadata }, _buyEvent_initializers, _buyEvent_extraInitializers);
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        ChartComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return ChartComponent = _classThis;
}();
exports.ChartComponent = ChartComponent;
//# sourceMappingURL=chart.component.js.map