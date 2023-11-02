import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent {
  _appearance: string = 'design__default';
  @Input({required: false, alias: 'type'}) inputType: string = "";
  @Input()
    set appearance(name: string){
      switch(name.toLocaleLowerCase()) {
        case "default":
          this._appearance = "design__default"
          break;
        case "outline-grey":
          this._appearance = "design__outline-grey"
          break;
        case "disabled":
          this._appearance = "design__disabled"
          break;
      }
    }

  ngOnInit(): void {

  }
}
