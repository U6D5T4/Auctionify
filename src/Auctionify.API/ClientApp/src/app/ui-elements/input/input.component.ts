import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss']
})
export class InputComponent {
  @Input({required: true, alias: 'type'}) inputType!: string; 
  @Input({required: true, alias: 'placeholder'}) placeholderText!: string; 
}
