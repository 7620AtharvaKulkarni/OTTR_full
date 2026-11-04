import { _isNumberValue } from '@angular/cdk/coercion';
import { Component } from '@angular/core';
import { ICellEditorAngularComp } from 'ag-grid-angular';
import { ToastrService } from 'ngx-toastr';
import { OttrService } from 'src/app/services/ottr.service';
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: 'app-dropdown',
  templateUrl: './dropdown.component.html',
  styleUrls: ['./dropdown.component.css']
})
export class DropdownComponent implements ICellEditorAngularComp {

  dropDownData = {};
  selectedValue: string;
  isEditable = true;
  EditRole = false;
  dropDownParams: any;
  constructor(private toastrService: ToastrService, private ottrService: OttrService, private spinner: NgxSpinnerService) { }

  getValue() {
    if (this.dropDownData != null && this.dropDownData != {}) {
      return this.dropDownData[this.selectedValue];
    }
  }


  agInit(params): void {
    this.dropDownParams = params;
    this.isEditable = params.values.isEditable;
    this.EditRole = params.values.role;
    if (this.dropDownParams.colDef.field.indexOf("spendPlan_Region") > -1 && this.dropDownParams.colDef.headerName.indexOf("Region") > -1 && this.isEditable == true) {
      this.getSpendPlanRegionData();
    }
    else if (this.dropDownParams.colDef.field.indexOf("spendPlan_Country") > -1 && this.dropDownParams.colDef.headerName.indexOf("Country") > -1 && this.isEditable == true) {
      this.getSpendPlanCountryData();
    }
    else if (this.dropDownParams.colDef.field.indexOf("gesGroupId") > -1 && this.dropDownParams.colDef.headerName.indexOf("GES Group") > -1 && this.isEditable == true && this.ottrService.getAllOptionselected() == true) {
      this.dropDownData = this.dropDownParams.data.subgroupData;
    }
    else {
      this.dropDownData = params.values.dropDownValues;
    }
    if (this.dropDownParams.colDef.field.indexOf("attributeToId") > -1) {
      delete this.dropDownData["0"];
    }
    console.log(this.dropDownParams.data);
    console.log(this.dropDownData);
    for (const key in this.dropDownData) {
      if (this.dropDownData[key] === params.value) {
        this.selectedValue = key;
        break;
      }
    }
    console.log(this.dropDownData);   
    console.log(this.dropDownParams.data); 
    if (this.selectedValue === undefined) {
      this.selectedValue = params.value;
    }


    if (!this.isEditable && this.EditRole) {
      //  this.toastrService.error('You cannot edit multiple rows at the same time!', 'Error');
    }
  }

  // getGESSubGroupByGroupID(){
  //   this.spinner.show();
  //   this.dropDownData = {};            
  //   var obj:any={};
  //   this.ottrService.getAllGESGroupData().subscribe(data=>{
  //     //console.log(data);
  //     //console.log(this.dropDownParams.data.gesGroupId);
  //     for(var item of data)
  //     {
  //       if(item.gesSubGroup.filter(x=>x.subgroupId == this.dropDownParams.data.gesGroupId).length > 0)
  //       {
  //         var filteredGroup = item.gesSubGroup.filter(x=>x.subgroupId == this.dropDownParams.data.gesGroupId)[0];
  //         obj[filteredGroup.subgroupId] = filteredGroup.subgroupName;
  //         if(item.gesSubGroup.length > 1)
  //         {
  //           var otherValuesinSubgroup = item.gesSubGroup.filter(x=>x.subgroupId!= this.dropDownParams.data.gesGroupId);
  //           for(var othergroup of otherValuesinSubgroup)
  //           {
  //             obj[othergroup.subgroupId] = othergroup.subgroupName;
  //           }
  //         }
  //         break;
  //       }
  //     }

  //     this.dropDownData = obj;     
  //     this.spinner.hide();
  //   },err=>{
  //     this.spinner.hide();
  //   });
  // }



  getSpendPlanRegionData() {
    this.spinner.show();
    var temp: any = this.ottrService.allPoleData;
    //console.log('all region data', temp);
    //console.log(this.dropDownParams.data);
    var poleId = 0;
    if (_isNumberValue(this.dropDownParams.data.spendPlan_Pole) == false) {
      temp = temp.filter(x => x.poleID == this.dropDownParams.data.spendPlan_PoleId);
      if (temp.length > 0) {
        poleId = temp[0].poleID;
      }
    }
    else {
      poleId = this.dropDownParams.data.spendPlan_Pole;
    }
    if (poleId != 0) {
      setTimeout(() => {
        this.ottrService.getSpendPlanRegionData(poleId).subscribe(data => {
          this.spinner.hide();
          this.dropDownData = data;
          for (const key in this.dropDownData) {
            if (this.dropDownData[key] === this.dropDownParams.data.spendPlan_Region) {
              this.selectedValue = key;
              break;
            }
          }
        }, err => {
          this.spinner.hide();
          this.toastrService.error('Failed to get data from server', 'Error');
        });
      }, 200);
    }
    else {
      this.spinner.hide();
    }
  }


  getSpendPlanCountryData() {
    this.spinner.show();
    var temp: any = this.ottrService.allSpendPlanRegionData;
    //console.log("all country Data", temp);
    var regionId = 0;
    //console.log(this.dropDownParams.data);
    if (_isNumberValue(this.dropDownParams.data.spendPlan_Region) == false) {
      temp = temp.filter(x => x.regionID == this.dropDownParams.data.spendPlan_RegionId);
      if (temp.length > 0) {
        regionId = temp[0].regionID;
      }
    }
    else {
      regionId = this.dropDownParams.data.spendPlan_Region;
    }
    if (regionId != 0) {
      setTimeout(() => {
        this.ottrService.getSpendPlanCountryData(regionId).subscribe(data => {
          this.spinner.hide();
          this.dropDownData = data;
          for (const key in this.dropDownData) {
            if (this.dropDownData[key] === this.dropDownParams.data.spendPlan_Country) {
              this.selectedValue = key;
              break;
            }
          }
        }, err => {
          this.toastrService.error('Failed to get data from server', 'Error');
          this.spinner.hide();

        });
      }, 200);
    }
    else {
      this.spinner.hide();
    }
  }


  ValueChange(event) {
    setTimeout(() => {
      if (this.dropDownParams.colDef.field.indexOf("surveyRequest") > -1 && this.dropDownParams.colDef.headerName.indexOf("Survey Request") > -1) {
        this.dropDownParams.value = this.selectedValue;
        this.ottrService.triggerSurveyRequestChangeEvent(this.dropDownParams);
        if (this.selectedValue.trim() == "NA") {
          this.dropDownParams.data.spendPlan_Pole = null;
          this.dropDownParams.data.spendPlan_Country = null;
          this.dropDownParams.data.spendPlan_Region = null;
          this.dropDownParams.data.surveyType = null;
          this.dropDownParams.data.gesResourceId = null;
          this.dropDownParams.data.gesResourceName = null;
          this.dropDownParams.data.spendPlan_PoleId = null;
          this.dropDownParams.data.spendPlan_RegionId = null;
          this.dropDownParams.data.spendPlan_CountryId = null;
        }
        else {
          this.dropDownParams.data.surveyType = "Back Office";
          this.dropDownParams.data.gesResourceId = null;
          this.dropDownParams.data.gesResourceName = null;
        }
      }
    }, 200);
    if (this.dropDownParams.colDef.field.indexOf("surveyType") > -1 && this.dropDownParams.colDef.headerName.indexOf("Survey Type") > -1) {
      if (this.selectedValue.trim() == "Back Office") {
        this.dropDownParams.data.gesResourceId = null;
        this.dropDownParams.data.gesResourceName = null;
      }
    }
    if (this.dropDownParams.colDef.field.indexOf("gesGroupId") > -1 && this.dropDownParams.colDef.headerName.indexOf("GES Group") > -1) {
      this.dropDownParams.data.productId = null;
    }
    if (this.dropDownParams.colDef.field.indexOf("spendPlan_Pole") > -1 && this.dropDownParams.colDef.headerName.indexOf("Pole") > -1) {
      this.dropDownParams.data.spendPlan_Region = null;
      this.dropDownParams.data.spendPlan_Country = null;
      this.dropDownParams.data.spendPlan_RegionId = null;
      this.dropDownParams.data.spendPlan_CountryId = null;
      this.dropDownParams.data.spendPlan_PoleId = Number(this.selectedValue);
    }
    if (this.dropDownParams.colDef.field.indexOf("spendPlan_Region") > -1 && this.dropDownParams.colDef.headerName.indexOf("Region") > -1) {
      {
        this.dropDownParams.data.spendPlan_Country = null;
        this.dropDownParams.data.spendPlan_CountryId = null;
        this.dropDownParams.data.spendPlan_RegionId = Number(this.selectedValue);
      }
    }
    if (this.dropDownParams.colDef.field.indexOf("spendPlan_Country") > -1 && this.dropDownParams.colDef.headerName.indexOf("Country") > -1) {
      {
        this.dropDownParams.data.spendPlan_CountryId = Number(this.selectedValue);
      }
    }

  }

}






