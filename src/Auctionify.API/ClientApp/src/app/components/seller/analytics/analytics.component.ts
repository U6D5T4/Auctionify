import { Component, NgModule, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import {
    ApexAxisChartSeries,
    ApexChart,
    ApexXAxis,
    ApexStroke,
    ApexTooltip,
    ApexDataLabels,
    ApexNonAxisChartSeries,
    ApexFill,
    ApexLegend,
    ApexPlotOptions,
    ApexResponsive,
    ApexTheme,
    ApexTitleSubtitle,
    ApexYAxis,
    ApexStates,
    ApexGrid,
    ApexAnnotations,
} from 'ng-apexcharts';
import { Client } from 'src/app/web-api-client';

enum LotStatusColor {
    Draft = '#3768BA',
    PendingApproval = '#4673C0',
    Rejected = '#557FC5',
    Upcoming = '#648ACB',
    Active = '#7396D0',
    Sold = '#81A1D6',
    NotSold = '#90ADDB',
    Cancelled = '#9FB8E1',
    Reopened = '#AEC4E6',
    Archive = '#BDCFEC',
}

const lotStatusOrder = [
    'Draft',
    'PendingApproval',
    'Rejected',
    'Upcoming',
    'Active',
    'Sold',
    'NotSold',
    'Cancelled',
    'Reopened',
    'Archive',
];

const FONT_FAMILY = 'Onest';
const COLOR_MAIN = '#2B5293';
const COLOR_NEUTRAL_100 = '#FFFFFF';

export type ChartOptions = {
    series: ApexAxisChartSeries;
    chart: ApexChart;
    xaxis: ApexXAxis;
    stroke: ApexStroke;
    tooltip: ApexTooltip;
    dataLabels: ApexDataLabels;
    plotOptions: ApexPlotOptions;
    annotations: ApexAnnotations;
    grid: ApexGrid;
    fill: ApexFill;
};

export type BarChartOptions = {
    series: ApexAxisChartSeries;
    chart: ApexChart;
    dataLabels: ApexDataLabels;
    plotOptions: ApexPlotOptions;
    yaxis: ApexYAxis;
    xaxis: ApexXAxis;
    fill: ApexFill;
    title: ApexTitleSubtitle;
    states: ApexStates;
    grid: ApexGrid;
};

export type PolarChartOptions = {
    series: ApexNonAxisChartSeries;
    chart: ApexChart;
    responsive: ApexResponsive[];
    labels: any;
    theme: ApexTheme;
    title: ApexTitleSubtitle;
    fill: ApexFill;
    yaxis: ApexYAxis;
    stroke: ApexStroke;
    legend: ApexLegend;
    plotOptions: ApexPlotOptions;
    tooltip: ApexTooltip;
    dataLabels: ApexDataLabels;
};

export interface TotalIncome {
    currency: string;
    totalIncome: number;
}

@Component({
    selector: 'app-analytics',
    templateUrl: './analytics.component.html',
    styleUrls: ['./analytics.component.scss'],
})
export class AnalyticsComponent implements OnInit {
    public chartOptions: Partial<ChartOptions> | null;
    public polarChartOptions: Partial<PolarChartOptions> | null;
    public barChartOptions: Partial<BarChartOptions> | null;

    public barPeriod: string = 'Total';
    public barPeriodNum: number = 1;
    public selectedBarPeriod: string = 'Total|1'; // Default bar selected period

    public linePeriod: string = 'Total'; // Default period
    public linePeriodNum: number = 1; // Default period number
    public selectedLinePeriod: string = 'Total|1'; // Default line selected period
    public totalIncome: TotalIncome[] = [];

    public selectedExportType: string | null = null;

    constructor(private client: Client) {
        this.chartOptions = null;
        this.barChartOptions = null;
        this.polarChartOptions = null;
    }

    ngOnInit(): void {
        this.constructBarChart();
        this.constructPolarChart();
        this.constructLineChart();
    }

    constructPolarChart() {
        this.client.getLotsStatuses().subscribe({
            next: (res) => {
                const totalCount = res.reduce(
                    (acc, curr) => acc + curr.count,
                    0
                );

                this.polarChartOptions = {
                    series: res
                        .sort((a, b) => a.count - b.count)
                        .map((d) => d.count),
                    chart: {
                        width: '80%',
                        type: 'donut',
                        foreColor: '#000',
                        fontFamily: FONT_FAMILY,
                    },
                    labels: res
                        .sort((a, b) => a.count - b.count)
                        .map((d) => d.status),

                    dataLabels: {
                        style: {
                            fontSize: '12px',
                            fontFamily: FONT_FAMILY,
                        },
                    },
                    fill: {
                        colors: lotStatusOrder.map(
                            (status) =>
                                LotStatusColor[
                                    status as keyof typeof LotStatusColor
                                ]
                        ),
                    },
                    yaxis: {
                        show: false,
                    },
                    legend: {
                        position: 'right',
                        markers: {
                            fillColors: lotStatusOrder.map(
                                (status) =>
                                    LotStatusColor[
                                        status as keyof typeof LotStatusColor
                                    ]
                            ),
                        },
                        fontSize: '16px',
                        fontFamily: FONT_FAMILY,
                    },
                    tooltip: {
                        fillSeriesColor: false,
                        marker: {
                            show: false,
                        },
                    },
                    plotOptions: {
                        pie: {
                            donut: {
                                labels: {
                                    show: true,
                                    name: {
                                        offsetY: 10,
                                    },
                                    total: {
                                        show: true,
                                        showAlways: true,
                                        fontSize: '36px',
                                        fontFamily: FONT_FAMILY,
                                        fontWeight: 600,
                                        label:
                                            totalCount +
                                            ' lot' +
                                            (totalCount > 1 ? 's' : ''),
                                        color: '#000',
                                        formatter: function (w) {
                                            return '';
                                        },
                                    },
                                },
                            },
                        },
                    },
                };
            },
        });
    }

    constructBarChart() {
        this.client
            .getCreatedLotsCount(this.barPeriod, this.barPeriodNum)
            .subscribe({
                next: (res) => {
                    this.barChartOptions = {
                        series: [
                            {
                                name: 'Created lots',
                                data: res.data.map((d) => d.count),
                            },
                        ],
                        chart: {
                            width: '85%',
                            type: 'bar',
                            fontFamily: FONT_FAMILY,
                        },
                        plotOptions: {
                            bar: {
                                dataLabels: {
                                    position: 'top',
                                },
                                borderRadius: 8,
                            },
                        },
                        dataLabels: {
                            enabled: true,
                            style: {
                                fontSize: '12px',
                                colors: [COLOR_NEUTRAL_100],
                                fontFamily: FONT_FAMILY,
                            },
                        },
                        xaxis: {
                            categories: res.data.map((d) =>
                                new Date(d.date).toLocaleDateString('en-US', {
                                    day: 'numeric',
                                    month: 'short',
                                    year: 'numeric',
                                })
                            ),
                            position: 'top',
                            axisBorder: {
                                show: false,
                            },
                            axisTicks: {
                                show: false,
                            },
                            crosshairs: {
                                fill: {
                                    type: 'gradient',
                                    gradient: {
                                        colorFrom: '#D8E3F0',
                                        colorTo: '#BED1E6',
                                        stops: [0, 100],
                                        opacityFrom: 0.4,
                                        opacityTo: 0.5,
                                    },
                                },
                            },
                            tooltip: {
                                enabled: false,
                            },
                        },
                        states: {
                            active: {
                                filter: {
                                    type: 'darken',
                                    value: 0.65,
                                },
                            },
                        },
                        fill: {
                            colors: ['#3768BA'],
                        },
                        yaxis: {
                            axisBorder: {
                                show: false,
                            },
                            axisTicks: {
                                show: false,
                            },
                            labels: {
                                show: false,
                                formatter: function (val) {
                                    return val.toFixed(0);
                                },
                                style: {
                                    fontFamily: FONT_FAMILY,
                                },
                            },
                        },
                        grid: {
                            show: false,
                        },
                    };
                },
            });
    }

    constructLineChart() {
        this.client
            .getUserIncome(this.linePeriod, this.linePeriodNum)
            .subscribe({
                next: (res) => {
                    this.totalIncome = res.reduce((acc: any, curr: any) => {
                        const index = acc.findIndex(
                            (item: any) => item.currency === curr.currency
                        );
                        if (index !== -1) {
                            acc[index].totalIncome += curr.amount;
                        } else {
                            acc.push({
                                currency: curr.currency,
                                totalIncome: curr.amount,
                            });
                        }
                        return acc;
                    }, []);

                    this.chartOptions = {
                        series: [
                            {
                                name: 'Income',
                                data: res.map((d) => ({
                                    x: new Date(d.date).getTime(),
                                    y: d.amount,
                                })),
                                color: '#8AADE8',
                            },
                        ],
                        chart: {
                            type: 'area',
                            toolbar: {
                                show: false,
                            },
                            fontFamily: FONT_FAMILY,
                        },
                        dataLabels: {
                            enabled: false,
                            style: {
                                fontFamily: FONT_FAMILY,
                            },
                        },
                        stroke: {
                            curve: 'smooth',
                            colors: [COLOR_MAIN],
                        },
                        xaxis: {
                            type: 'datetime',
                            categories: res.map((d) =>
                                new Date(d.date).toDateString()
                            ),
                            axisTicks: {
                                show: false,
                            },
                            tooltip: {
                                enabled: false,
                            },
                        },
                        tooltip: {
                            x: {
                                format: 'MMM dd, yyyy',
                            },
                        },
                        grid: {
                            show: false,
                        },
                        annotations: {
                            points: res.map((d) => {
                                return {
                                    x: new Date(d.date).getTime(),
                                    y: d.amount,
                                    marker: {
                                        size: 6,
                                        fillColor: COLOR_NEUTRAL_100,
                                        strokeColor: COLOR_MAIN,
                                        strokeWidth: 3,
                                        fontFamily: FONT_FAMILY,
                                    },
                                    label: {
                                        borderColor: COLOR_MAIN,
                                        style: {
                                            color: COLOR_NEUTRAL_100,
                                            background: COLOR_MAIN,
                                            fontFamily: FONT_FAMILY,
                                        },
                                    },
                                };
                            }),
                        },
                        fill: {
                            type: 'gradient',
                            gradient: {
                                shadeIntensity: 1,
                                inverseColors: false,
                                opacityFrom: 0.8,
                                opacityTo: 0.4,
                                stops: [0, 100],
                            },
                        },
                    };
                },
            });
    }

    onLineChartPeriodChange(selectedValue: string) {
        const [period, periodNumString] = selectedValue.split('|');
        const periodNum = parseInt(periodNumString);

        this.filterLineChartNetIncome(period, periodNum);
    }

    filterLineChartNetIncome(period: string, periodNum: number) {
        this.linePeriod = period;
        this.linePeriodNum = periodNum;
        this.constructLineChart();
    }

    onExportFileSelect(selectedExportType: string) {
        console.log(selectedExportType);
    }

    onBarChartPeriodChange(selectedValue: string) {
        const [period, periodNumString] = selectedValue.split('|');
        const periodNum = parseInt(periodNumString);

        this.filterBarChart(period, periodNum);
    }

    filterBarChart(period: string, periodNum: number) {
        this.barPeriod = period;
        this.barPeriodNum = periodNum;
        this.constructBarChart();
    }
}
