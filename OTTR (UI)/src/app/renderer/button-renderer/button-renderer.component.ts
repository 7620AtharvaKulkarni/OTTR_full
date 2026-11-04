import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-button-renderer',
  templateUrl: './button-renderer.component.html',
  styleUrls: ['./button-renderer.component.css']
})
export class ButtonRendererComponent implements ICellRendererAngularComp {

  constructor() { }

  params;
  isVisible: boolean;
  label: string;
  rowIndex: number;
  isDisabled = false;

  agInit(params): void {
    this.params = params;
    this.label = this.params.label || null;
    this.rowIndex = this.params.rowIndex;
    this.isVisible = this.params.isVisible;
    this.isDisabled = this.params.isDisabled;
  }

  refresh(params?: any): boolean {
    return true;
  }

  onClick($event) {
    if (this.params.onClick instanceof Function) {
      const params = {
        event: $event,
        rowIndex: this.rowIndex,
        rowNode: this.params.node
      }
      this.params.onClick(params);

    }
  }

}
