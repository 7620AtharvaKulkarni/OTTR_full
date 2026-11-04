import { Component, ViewChild, ViewContainerRef, AfterViewInit } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { OttrService } from 'src/app/services/ottr.service';

@Component({
  selector: 'app-textarea',
  templateUrl: './textarea.component.html',
  styleUrls: ['./textarea.component.css']
})
export class TextareaComponent implements ICellEditorAngularComp, AfterViewInit {

  @ViewChild('textArea', { read: ViewContainerRef }) textarea;
  data = "";
  maxlength = 50;
  isEditable = true;
  EditRole = '';
  textAreaParams: any;
  constructor(private toastrService: ToastrService,
    private ottrService: OttrService,
    private spinner: NgxSpinnerService) { }

  ngAfterViewInit() {
    window.setTimeout(() => {
      this.textarea.element.nativeElement.focus();
    });
  }

  agInit(params): void {
    this.data = params.value != undefined ? params.value : null;
    this.maxlength = params.values.maxlength;
    this.isEditable = params.values.isEditable;
    this.EditRole = params.values.role;
    this.textAreaParams = params;
    if (!this.isEditable && this.EditRole) {
      //  this.toastrService.error('You cannot edit multiple rows at the same time!', 'Error');
    }
  }

  getValue() {
    return this.data;
  }

  keyPressedEvent(event) {
    if (this.data !== null && this.data.length >= this.maxlength) {
      this.toastrService.error('Max Length reached!', 'Error');
    }    
    if(this.textAreaParams.colDef.headerName == 'Delivery Hours' || this.textAreaParams.colDef.headerName == 'Rework Hours')
    {
      const keyPressed = event.key;
      const isValidKey = /^[0-9.]$/.test(keyPressed) || ['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(keyPressed);
      if (!isValidKey) {
        event.preventDefault();
      }
    }
  }

  valueChange(event){
    if (this.textAreaParams.colDef.field.indexOf("gesResourceId") > -1) {
      if(event.length == 0)
      {
        this.textAreaParams.data.gesResourceName = null;
        this.textAreaParams.api.refreshCells();
        this.textAreaParams.api.redrawRows();
      }
      if (event.length == 7) {
        this.spinner.show();
        this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(event).subscribe(res => {
          if(res == null){
            this.spinner.hide();
            return;
          } 
          if(res!=undefined && res!= null 
            && res.data!=undefined && res.data!=null)
          {            
            this.textAreaParams.data.gesResourceName = res.data;                        
            this.textAreaParams.api.redrawRows();
            this.textAreaParams.api.refreshCells();
            this.textAreaParams.api.clearFocusedCell();            
          }
          else
          {
            this.toastrService.error('GES Resource EID not found', 'Error');          
            this.textAreaParams.data.gesResourceId = null;
            this.textAreaParams.data.gesResourceName = null;
            this.clearGESResource();            
          }
          this.spinner.hide();
        }, err => {
          this.toastrService.error('Server Error while finding name', 'Error');                    
          this.spinner.hide();
        });
      }      
    }
    if (this.textAreaParams.colDef.field.indexOf("gespmEID") > -1) {      
      if (event.length == 7) {
        this.spinner.show();
        this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(event).subscribe(res => {
          if(res == null){
            this.spinner.hide();
            return;

          } 
          if(res!=undefined && res!= null 
            && res.data!=undefined && res.data!=null)
          { 
            this.textAreaParams.data.gespm = res.data;                        
            this.textAreaParams.api.redrawRows();
            this.textAreaParams.api.refreshCells();
            this.textAreaParams.api.clearFocusedCell();            
          }
          else
          {
            this.toastrService.error('GES PM/EM not found', 'Error');          
            this.textAreaParams.data.gespm = null;     
            this.textAreaParams.data.gespmEID = null;      
            this.clearGESPM();             
          }
          this.spinner.hide();
        }, err => {
          this.toastrService.error('Server Error while finding name', 'Error');          
          this.spinner.hide();
        });
      }      
    }
  }

  clearGESPM(){    
    return this.data=null;
  }

  clearGESResource(){    
    return this.data=null;
  }

  keydownEvent(event){
    if(event.key=="Backspace" || event.key=="Delete")
    {
      if(this.textAreaParams.colDef.field=="gespmEID" || this.textAreaParams.colDef.field=="gesResourceId")
      {
        event.preventDefault();
      }
    }
  }

  clearFOPM(){    
    return this.data=null;
  }

  
 
  pasteEvent(event: ClipboardEvent) {
    const pastedText = event.clipboardData.getData('text');
    if (pastedText.length >= this.maxlength) {
      this.toastrService.error('Max Length reached!', 'Error');
    }    
  }


  validateFOPMNames()
  {
    var fopmNameArray = this.data.split(";");
    if(fopmNameArray.length > 5)
    {
      this.toastrService.error("Maximum 5 FOPM EID are allowed", "Error");
      return false;
    }
    for(var item of fopmNameArray)
    {
      var duplicateCount = fopmNameArray.filter(x=>x.toLowerCase().trim() == item.toLowerCase().trim()).length;
      if(duplicateCount > 1)
      {
        this.toastrService.error("Duplicate EID are not allowed","Error");
        return false;
      }
      if(item.length!=7)
      {
        this.toastrService.error("Please enter all FOPM ID of 7 Characters if any ID in the list is invalid then all names will not be found","Error");
        return false;
      }
    }
    return true;
  }


  searchFOPM(){
    if (this.textAreaParams.colDef.field.indexOf("fopmId") > -1) {
      if(this.data.length == 0)
      {
        this.textAreaParams.data.fopmName = null;
        this.textAreaParams.api.refreshCells();
        this.textAreaParams.api.redrawRows();
      }
      if (this.data.length > 0) {
        if(this.validateFOPMNames() == false) return;
        this.spinner.show();
        this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(this.data).subscribe(res => {
          if(res == null){
            this.spinner.hide();
            return;
          } 
          if(res!=undefined && res!= null 
            && res.data!=undefined && res.data!=null )
          {
            this.textAreaParams.data.fopmName = res.data;                                   
            this.textAreaParams.api.refreshCells();
            this.textAreaParams.api.clearFocusedCell();            
            this.textAreaParams.api.redrawRows();        
            this.textAreaParams.api.resetRowHeights();    
          }
          else
          {
            this.toastrService.error('FOPM EID not found. Please check if you have entered valid EID', 'Error');          
            //this.textAreaParams.data.fopmId = null;
            this.textAreaParams.data.fopmName = null;
            //this.clearFOPM();            
          }
          this.spinner.hide();
        }, err => {
          this.toastrService.error('Server Error while finding name', 'Error');                    
          this.spinner.hide();
        });
      }      
    }    
  }

}
