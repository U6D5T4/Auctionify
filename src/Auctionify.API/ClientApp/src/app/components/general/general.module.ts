import { NgModule } from '@angular/core';
import { ProfileModule } from './profile/profile.module';
import { GeneralRoutingModule } from './general-routing.module';


@NgModule({
  imports: [
    ProfileModule,
    GeneralRoutingModule,
  ],

  declarations: [
  ],
  
  exports: [
    ProfileModule
  ]
})
export class GeneralModule { }
