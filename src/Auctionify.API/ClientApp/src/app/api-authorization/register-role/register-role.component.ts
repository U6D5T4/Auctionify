import { Component, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})

@Component({
  selector: 'app-register-role',
  templateUrl: './register-role.component.html',
  styleUrls: ['./register-role.component.scss']
})
export class RegisterRoleComponent {
  selectedType: string = '';
  isLoading = false;

  selectUserType(userType: string) {
    this.selectedType = userType;
  }
}
