import { Component } from '@angular/core';
import { IHeaderAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-custom-header',
  templateUrl: './custom-header.component.html',
  styleUrls: ['./custom-header.component.css']
})
export class CustomHeaderComponent implements IHeaderAngularComp {

  isRequired: boolean;
  headerLabel: string;

  constructor() { }

  agInit(params): void {
    this.headerLabel = params.displayName;
    this.isRequired = params.isRequired;
  }
}
