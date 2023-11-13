import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilesPopUpComponent } from './files.component';

describe('FilesComponent', () => {
    let component: FilesPopUpComponent;
    let fixture: ComponentFixture<FilesPopUpComponent>;

    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [FilesPopUpComponent],
        });
        fixture = TestBed.createComponent(FilesPopUpComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
