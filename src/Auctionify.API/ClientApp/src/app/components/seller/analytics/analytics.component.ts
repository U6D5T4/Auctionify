import { Component, OnInit } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexXAxis, ApexStroke, ApexTooltip, ApexDataLabels, ApexNonAxisChartSeries, ApexFill, ApexLegend, ApexPlotOptions, ApexResponsive, ApexTheme, ApexTitleSubtitle, ApexYAxis, ApexStates, ApexGrid } from 'ng-apexcharts';
import { Client } from 'src/app/web-api-client';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  stroke: ApexStroke;
  tooltip: ApexTooltip;
  dataLabels: ApexDataLabels;
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
  grid: ApexGrid
};

export type PolarChartOptions = {
  series: ApexNonAxisChartSeries;
  chart: ApexChart;
  responsive: ApexResponsive[];
  labels: any;
  theme: ApexTheme;
  title: ApexTitleSubtitle;
  fill: ApexFill,
  yaxis: ApexYAxis,
  stroke: ApexStroke,
  legend: ApexLegend,
  plotOptions: ApexPlotOptions,
  tooltip: ApexTooltip,
  dataLabels: ApexDataLabels
};

@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.scss']
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
    this.barChartOptions = null
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
        this.polarChartOptions = {
          series: res.sort((a, b) => a.count - b.count).map(d => d.count),
          chart: {
            width: '80%',
            type: 'donut',
            foreColor: '#000'
          },
          labels: res.sort((a, b) => a.count - b.count).map(d => d.status),
          fill: {
            colors: ['#2b5293', '#40639e', '#5575a9', '#6b86b3', '#8097be', '#95a9c9', '#aabad4', '#bfcbdf', '#d5dce9', '#eaeef4']
          },
          yaxis: {
            show: false
          },
          legend: {
            position: 'bottom',
            markers: {
              fillColors: ['#2b5293', '#40639e', '#5575a9', '#6b86b3', '#8097be', '#95a9c9', '#aabad4', '#bfcbdf', '#d5dce9', '#eaeef4']
            }
          },
          tooltip: {
            fillSeriesColor: false,
            marker: {
              show: false
            }
          }
        };
      }
    })
  }

  constructBarChart() {
    this.client.getCreatedLotsCount(this.barPeriod, this.barPeriodNum)
      .subscribe({
        next: (res) => {
          this.barChartOptions = {
            series: [
              {
                name: "Created lots",
                data: res.data.map(d => d.count)
              }
            ],
            chart: {
              width: '80%',
              type: "bar"
            },
            plotOptions: {
              bar: {
                dataLabels: {
                  position: "top" // top, center, bottom
                }
              }
            },
            dataLabels: {
              enabled: true,
              offsetY: -20,
              style: {
                fontSize: "12px",
                colors: ["#304758"]
              }
            },
            xaxis: {
              categories: res.data.map(d => (new Date(d.date)).toDateString()),
              position: "top",
              axisBorder: {
                show: false
              },
              axisTicks: {
                show: false
              },
              crosshairs: {
                fill: {
                  type: "gradient",
                  gradient: {
                    colorFrom: "#D8E3F0",
                    colorTo: "#BED1E6",
                    stops: [0, 100],
                    opacityFrom: 0.4,
                    opacityTo: 0.5
                  }
                }
              },
              tooltip: {
                enabled: true,
                offsetY: -35
              }
            },
            states: {
              active: {
                filter: {
                  type: 'darken',
                  value: 0.65
                }
              }
            },
            fill: {
              colors: ['#3768BA']
            },
            yaxis: {
              axisBorder: {
                show: false
              },
              axisTicks: {
                show: false
              },
              labels: {
                show: false,
                formatter: function (val) {
                  return val + "%";
                }
              }
            },
            grid: {
              show: false
            }
          };
        }
      })
  }

  constructLineChart() {
    this.client.getUserIncome(this.linePeriod, this.linePeriodNum).subscribe({
      next: (res) => {
        this.chartOptions = {
          series: [{
            name: 'main',
            data: res.map(d => d.amount)
          }],
          chart: {
            type: "area",
            toolbar: {
              show: false,
            },
          },
          dataLabels: {
            enabled: false
          },
          stroke: {
            curve: "smooth"
          },
          xaxis: {
            type: "datetime",
            categories: res.map(d => (new Date(d.date)).toDateString())
          },
          tooltip: {
            x: {
              format: "dd/MM/yy HH:mm"
            }
          }
        };
      }
    })
  }
}
