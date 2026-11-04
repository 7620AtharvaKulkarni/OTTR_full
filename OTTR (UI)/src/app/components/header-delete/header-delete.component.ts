import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { IHeaderAngularComp } from 'ag-grid-angular';
import { IAfterGuiAttachedParams, IHeaderParams } from 'ag-grid-community';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { OttrService } from 'src/app/services/ottr.service';
import { ConfirmationModalComponent } from '../confirmation-modal/confirmation-modal.component';

@Component({
  selector: 'app-header-delete',
  templateUrl: './header-delete.component.html',
  styleUrls: ['./header-delete.component.scss']
})
export class HeaderDeleteComponent implements IHeaderAngularComp {
  title: any;
  selectAll = false;
  isDisabled = true;
  constructor(
    private toastr: ToastrService,
    private ottrService: OttrService,
    private spinner: NgxSpinnerService,
    private dialog: MatDialog,
  ) {


  }
  refresh(params: IHeaderParams): boolean {
    return false;
  }
  agInit(params: IHeaderParams): void {
    this.title = params['title'];

  }


  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
  }

  deleteOTTR() {
    if (this.ottrService.ottrDeleteId.length > 0) {
      console.log(this.ottrService.ottrDeleteId);
      const dialogRef = this.dialog.open(ConfirmationModalComponent, {
        width: '400px',
        data: {
          message: "Do you want to delete the records?",
          actionButton: 'Yes'
        },
        disableClose: true
      });
      dialogRef.afterClosed().subscribe(result => {
        if (result == true) {
          this.spinner.show();
          var deleteData: any = {};
          deleteData.ottrIds = this.ottrService.ottrDeleteId;
          console.log(deleteData);
          this.ottrService.deleteOttrMonitoringData(deleteData).subscribe((data: any) => {            
            if (data.StatusCode == 401 && data.data == "unauthorized access") {
              this.spinner.hide();
              window.location.href = data.URL;
            } else if (data.Status == 401 && data.data == "unauthorized access") {
              this.spinner.hide();
              window.location.href = data.URL;
            } else if (data.StatusCode == 400) {
              this.spinner.hide();
              this.toastr.error(JSON.parse(data.data).message, 'Error');
            }
            else {
              this.ottrService.triggerUploadExcelEvent("test");
              this.toastr.success('OTTR records deleted successfully', 'Done');
              this.ottrService.ottrDeleteId=[];
            }
          }, err => {
            this.spinner.hide();
            this.toastr.error('Failed to delete data from server!', 'Error');
            console.error(err);
          });
        } else {
          this.spinner.hide();
        }
      });
    }
    else {
      this.spinner.hide();
      this.toastr.error("Please select atleast one record.", "Error");
    }
  }



}