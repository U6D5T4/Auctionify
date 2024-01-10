import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Dialog } from '@angular/cdk/dialog';

import { DialogPopupComponent } from 'src/app/ui-elements/dialog-popup/dialog-popup.component';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { FileModel } from 'src/app/models/fileModel';
import {
    BuyerModel,
    SellerModel,
    UpdateUserProfileModel,
} from 'src/app/models/users/user-models';
import { Client } from 'src/app/web-api-client';
import { RatePaginationModel } from 'src/app/models/rates/rate-models';

export interface ProfileFormModel {
    firstName: FormControl<string | null>;
    lastName: FormControl<string | null>;
    phoneNumber: FormControl<string | null>;
    aboutMe: FormControl<string | null>;
    profilePicture: FormControl<FileModel | null>;
    deleteProfilePicture: FormControl<boolean | null>;
}

@Component({
    selector: 'app-update-user-profile',
    templateUrl: './update-user-profile.component.html',
    styleUrls: ['./update-user-profile.component.scss'],
})
export class UpdateUserProfileComponent {
    userProfileData: BuyerModel | SellerModel | null = null;
    isLoading = false;

    constructor(
        private authorizeService: AuthorizeService,
        private client: Client,
        private router: Router,
        public dialog: Dialog
    ) {}

    ngOnInit(): void {
        this.fetchUserProfileData();
    }

    profileForm = new FormGroup<ProfileFormModel>({
        firstName: new FormControl<string>('', [
            Validators.required,
            Validators.pattern(/^[a-zA-Z]*$/),
            Validators.maxLength(30),
            this.noSpaceValidator.bind(this),
        ]),
        lastName: new FormControl<string>('', [
            Validators.required,
            Validators.pattern(/^[a-zA-Z]*$/),
            Validators.maxLength(30),
            this.noSpaceValidator.bind(this),
        ]),
        aboutMe: new FormControl<string>('', [
            Validators.required,
            Validators.minLength(30),
        ]),
        phoneNumber: new FormControl<string>('', [
            Validators.required,
            Validators.pattern(/^\+\d{1,15}$/),
        ]),
        profilePicture: new FormControl<FileModel | null>(null),
        deleteProfilePicture: new FormControl<boolean>(false),
    });

    toggleDeleteProfilePicture() {
        this.profileForm.controls.deleteProfilePicture.setValue(
            !this.profileForm.controls.deleteProfilePicture.value
        );
    }

    onFileChange(event: any) {
        const fileInput = event.target;

        if (fileInput.files && fileInput.files.length > 0) {
            const file = fileInput.files[0];
            const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];

            if (allowedTypes.includes(file.type)) {
                const fileModel: FileModel = {
                    name: file.name,
                    file,
                    fileUrl: URL.createObjectURL(file),
                };
                if (this.userProfileData) {
                    this.userProfileData.profilePictureUrl = null;
                }
                this.profileForm.controls.profilePicture.setValue(fileModel);
                this.profileForm.controls.deleteProfilePicture.setValue(false);
            } else {
                this.openDialog(
                    ['Something went wrong, please try again later'],
                    true
                );
            }
        } else {
            this.profileForm.controls.profilePicture.setValue(null);
        }
    }

    noSpaceValidator(control: FormControl): { [key: string]: any } | null {
        if (this.hasSpace(control.value)) {
            return { noSpace: true };
        }
        return null;
    }

    hasSpace(value: string | null): boolean {
        return value !== null && /\s/.test(value);
    }

    openDialog(text: string[], error: boolean) {
        const dialogRef = this.dialog.open<string>(DialogPopupComponent, {
            data: {
                text,
                isError: error,
            },
        });

        dialogRef.closed.subscribe((res) => {});
    }

    private fetchUserProfileData() {
        const pagination: RatePaginationModel = {
            pageIndex: 0,
            pageSize: 10,
        };

        if (this.isUserBuyer()) {
            this.client.getBuyer(pagination).subscribe(
                (data: BuyerModel) => {
                    this.setFormControlData(data);
                    this.validate();
                },
                (error) => {
                    this.openDialog(
                        error.errors! || [
                            'Something went wrong, please try again later',
                        ],
                        true
                    );
                }
            );
        } else if (this.isUserSeller()) {
            this.client.getSeller(pagination).subscribe(
                (data: SellerModel) => {
                    this.setFormControlData(data);
                    this.validate();
                },
                (error) => {
                    this.openDialog(
                        error.errors! || [
                            'Something went wrong, please try again later',
                        ],
                        true
                    );
                }
            );
        }
    }

    getAverageStars(rate: number | null): string[] {
        const averageRating = rate;

        const roundedAverage = Math.round(averageRating!);

        const stars: string[] = [];
        for (let i = 1; i <= 5; i++) {
            if (i <= roundedAverage) {
                stars.push('star');
            } else if (i - roundedAverage === 0.5) {
                stars.push('star_half');
            } else {
                stars.push('star_border');
            }
        }

        return stars;
    }

    private validate() {
        if (!this.userProfileData?.averageRate) {
            this.userProfileData!.averageRate = 0;
        } else if (!this.userProfileData?.ratesCount) {
            this.userProfileData!.ratesCount = 0;
        }
    }

    setFormControlData(
        userProfileData: BuyerModel | SellerModel | null = null
    ) {
        if (userProfileData) {
            this.userProfileData = userProfileData;
            this.profileForm.setValue({
                firstName: userProfileData.firstName || '',
                lastName: userProfileData.lastName || '',
                aboutMe: userProfileData.aboutMe || '',
                phoneNumber: userProfileData.phoneNumber || '',
                profilePicture: null,
                deleteProfilePicture: false,
            });
        }
    }

    OnSubmit() {
        this.isLoading = true;

        if (this.profileForm.invalid) {
            this.isLoading = false;
            return;
        }

        const formValue = this.profileForm.value;

        const updateProfileModel: UpdateUserProfileModel = {
            firstName: formValue.firstName || null,
            lastName: formValue.lastName || null,
            phoneNumber: formValue.phoneNumber || null,
            aboutMe: formValue.aboutMe || null,
            profilePicture: formValue.profilePicture
                ? formValue.profilePicture.file
                : null,
            deleteProfilePicture: !!formValue.deleteProfilePicture,
        };

        this.client.updateProfile(updateProfileModel).subscribe({
            next: (result) => {
                if (result) {
                    this.router.navigate(['/profile']);
                }
            },
            complete: () => {
                this.isLoading = false;
            },
        });
    }

    isUserSeller(): boolean {
        return this.authorizeService.isUserSeller();
    }

    isUserBuyer(): boolean {
        return this.authorizeService.isUserBuyer();
    }
}
