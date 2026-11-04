import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { environment } from 'src/environments/environment';
import * as _ from 'lodash';
import { ToastrService } from 'ngx-toastr';
import { OttrService } from './../services/ottr.service';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'upload-dialog-box',
  templateUrl: './upload-dialog.component.html',
  styleUrls: ['./upload-dialog.component.scss']
})

export class UploadDialogComponent {


  acceptFiles = environment.acceptFile;
  uploadedfilelist: any = [];
  inputFile: any;
  isFile = true;

  constructor(private ottrService: OttrService, public dialogRef: MatDialogRef<UploadDialogComponent>, private toaster: ToastrService, private spinner: NgxSpinnerService) {

  }

  uploadFile(event) {
    this.uploadedfilelist = [];
    _.forEach(event.target.files, (file) => {
      const isAvailable = _.find(this.uploadedfilelist, (o) => {
        return o.name === file.name;
      });
      if (!isAvailable) {
        this.uploadedfilelist.push(file);
        this.isFile = false;
      } else {

      }
    });

  }
  upload() {
    this.spinner.show();
    if (this.uploadedfilelist != null) {
      this.isFile = true;
      this.ottrService.uploadFiles(this.uploadedfilelist).subscribe(res => {        
        this.spinner.hide();
        this.dialogRef.close(false);
        if (res.Status === 401) {
          if (res.data === 'unauthorized access') {
            setTimeout(() => {
              window.location.href = res.URL;
            }, 100);
          }
          this.dialogRef.close(false);
        } else {
          const statusCode = JSON.parse(res.StatusCode);
          if (statusCode === 200) {
            const data = JSON.parse(res.data);
            if(data.code == 402 && data.data.indexOf("malicious") > -1)
            {
              this.toaster.error(data.data, 'Error');
              this.dialogRef.close(false);
              return;
            }
            if(data.code == 400)
            {
              this.toaster.error(data.data, 'Error',{disableTimeOut:true});
              this.dialogRef.close(false);
              return;
            }
            this.toaster.success('successfully uploaded', 'Done');
            var temp:any={};
            temp.status =true;
            this.dialogRef.close(temp);

          } else {
            const resp = JSON.parse(res.data);
            if (res.StatusCode === 400) {
              this.toaster.error(resp.message, 'Error');
            }
            this.dialogRef.close(false);
          }
        }
      }, err => {
        this.spinner.hide();
        let message;
        if (err.error !== undefined && err.error != null && err.error.Message !== undefined
          && err.error.Message != null && err.error.Message !== '') {
          if (_.isString(err.error.Message)) {
            message = err.error.Message ? err.error.Message : 'something is wrong';
          }
        }
        //do not change this condition to uppercase,the  malicious file error  message  param is lowercase only
        if (err.error !== undefined && err.error != null && err.error.message !== undefined
          && err.error.message != null && err.error.message !== '') {
          if (_.isString(err.error.message)) {
            message = err.error.message ? err.error.message : 'something is wrong';
          }
        }
        else {
          if (err.statusText !== undefined && err.statusText != null && err.statusText !== '') {
            message = err.statusText;
          }
        }
        if (message === 'Unknown Error') {
          message = 'Cannot connect to server please check your network and try again';
        }

        this.toaster.error(message, 'Error');
        this.dialogRef.close(false);
      });





    }
  }
}