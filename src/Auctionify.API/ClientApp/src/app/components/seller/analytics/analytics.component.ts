import { Component, OnInit } from '@angular/core';
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
    Draft = '#2b5293',
    PendingApproval = '#40639e',
    Rejected = '#5575a9',
    Upcoming = '#6b86b3',
    Active = '#8097be',
    Sold = '#95a9c9',
    NotSold = '#aabad4',
    Cancelled = '#bfcbdf',
    Reopened = '#d5dce9',
    Archive = '#eaeef4',
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
};

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

    public linePeriod: string = 'Total';
    public linePeriodNum: number = 1;

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
                        fontFamily: 'Onest',
                    },
                    labels: res
                        .sort((a, b) => a.count - b.count)
                        .map((d) => d.status),
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
                        fontFamily: 'Onest',
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
                                        fontFamily: 'Onest',
                                        fontWeight: 600,
                                        label: totalCount.toString() + ' lots',
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
                            width: '80%',
                            type: 'bar',
                            fontFamily: 'Onest',
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
                                colors: ['#fff'],
                                fontFamily: 'Onest',
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
                                enabled: true,
                                offsetY: -35,
                                style: {
                                    fontFamily: 'Onest',
                                },
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
                                    return val + ' lot(s)';
                                },
                                style: {
                                    fontFamily: 'Onest',
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
                            fontFamily: 'Onest',
                        },
                        dataLabels: {
                            enabled: false,
                            style: {
                                fontFamily: 'Onest',
                            },
                        },
                        stroke: {
                            curve: 'smooth',
                            colors: ['#2B5293'],
                        },
                        xaxis: {
                            type: 'datetime',
                            categories: res.map((d) =>
                                new Date(d.date).toDateString()
                            ),
                        },
                        tooltip: {
                            x: {
                                format: 'dd/MM/yy HH:mm',
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
                                        fillColor: '#fff',
                                        strokeColor: '#2B5293',
                                        strokeWidth: 3,
                                        fontFamily: 'Onest',
                                    },
                                    label: {
                                        borderColor: '#2B5293',
                                        style: {
                                            color: '#fff',
                                            background: '#2B5293',
                                            fontFamily: 'Onest',
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
}
