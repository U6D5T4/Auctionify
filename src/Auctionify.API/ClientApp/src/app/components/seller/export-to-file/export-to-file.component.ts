import { DialogRef } from '@angular/cdk/dialog';
import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

import { Client } from 'src/app/web-api-client';

enum ExportPeriod {
    LastThreeMonths = '3',
    LastSixMonths = '6',
    LastTwelveMonths = '12',
}

enum ExportType {
    PDF = 'PDF',
    XLSX = 'XLSX',
}

@Component({
    selector: 'app-export-to-file',
    templateUrl: './export-to-file.component.html',
    styleUrls: ['./export-to-file.component.scss'],
})
export class ExportToFileComponent {
    public selectedExportPeriod: string = ExportPeriod.LastThreeMonths; // Default value
    public selectedExportType: string = ExportType.PDF; // Default value

    constructor(
        private dialogRef: DialogRef<ExportToFileComponent>,
        private apiClient: Client,
        private snackBar: MatSnackBar
    ) {}

    onExportSubmit() {
        this.downloadReport(
            parseInt(this.selectedExportPeriod),
            this.selectedExportType
        );
    }

    onExportFileTypeSelect(selectedExportType: string) {
        this.selectedExportType = selectedExportType;
    }

    onExportPeriodSelect(selectedExportPeriod: string) {
        this.selectedExportPeriod = selectedExportPeriod;
    }

    downloadReport(monthsDuration: number, reportType: string) {
        this.apiClient.downloadReport(monthsDuration, reportType).subscribe({
            next: (res) => {
                const blob = new Blob([res.data], {
                    type: 'application/octet-stream',
                });
                const url = window.URL.createObjectURL(blob);
                const a = document.createElement('a');
                a.href = url;
                a.download = res.filename;
                a.click();
                window.URL.revokeObjectURL(url);

                this.snackBar.open(
                    'Started downloading ' + res.filename,
                    'Close',
                    {
                        horizontalPosition: 'center',
                        verticalPosition: 'bottom',
                        duration: 5000,
                        panelClass: ['success-snackbar'],
                    }
                );
            },
            error: (error) => {
                this.snackBar.open('Failed to download report', 'Close', {
                    horizontalPosition: 'center',
                    verticalPosition: 'bottom',
                    duration: 5000,
                    panelClass: ['error-snackbar'],
                });
            },
        });
    }

    closeDialog() {
        this.dialogRef.close();
    }
}
