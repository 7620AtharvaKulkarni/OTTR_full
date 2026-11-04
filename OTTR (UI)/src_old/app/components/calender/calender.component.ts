import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { ICellEditorParams, ValueFormatterParams } from 'ag-grid-community';
import { MatDatepicker } from '@angular/material/datepicker';
import { ToastrService } from 'ngx-toastr';

@Component({
	selector: 'app-calender',
	templateUrl: './calender.component.html',
	styleUrls: ['./calender.component.css']
})

export class CalenderComponent implements AfterViewInit {
	cellEditorParams: ICellEditorParams;
	value: string;
	isEditable = true;
	EditRole=false;
	 label: string;
	@ViewChild('picker', { read: MatDatepicker }) picker: MatDatepicker<Date>;

	constructor(private toastrService: ToastrService) { }

	public static formatForDisplay(params: ValueFormatterParams): string {
		if (!params) {
			return '';
		}
		let date: Date;
		if (typeof params.value === 'number' || typeof params.value === 'string') {
			const stringValue = String(params.value);
			if (stringValue.length > 7 && /^[0-9]*$/.test(stringValue)) {
				const year: number = parseInt(stringValue.substring(0, 4), 10);
				const month: number = parseInt(stringValue.substring(4, 6), 10);
				const day: number = parseInt(stringValue.substring(6, 8), 10);
				date = new Date(year, month - 1, day);
			}
		} else {
			date = params.value;
		}
		return date ? date.toLocaleDateString() : '';
	}

	agInit(params): void {
	
		this.cellEditorParams = params;
		this.label = params.colDef.field || null;
		this.value = params.value;
		this.isEditable = params.values.isEditable;
		  this.EditRole= params.values.role;
      
        if (!this.isEditable && this.EditRole) {
		//	this.toastrService.error('You cannot edit multiple rows at the same time!', 'Error');
		}
	}

	getValue(): string {
		return this.value;
	}

	isCancelAfterEnd(): boolean {
		return false;
	}

	isCancelBeforeStart(): boolean {
		return false;
	}

resetForm(): string{
  return this.value = null;
}

	isPopup(): boolean {
		return false;
	}

	ngAfterViewInit() {
		this.picker.open();
	}

	onSelectChange(): void {
		setTimeout(() => {
			this.cellEditorParams.stopEditing();
		});
	}
}
