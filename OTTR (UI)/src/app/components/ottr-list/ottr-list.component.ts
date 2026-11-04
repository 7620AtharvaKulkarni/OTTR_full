import { Component, OnInit, Input, OnChanges, Output, EventEmitter } from '@angular/core';
import { OttrService } from 'src/app/services/ottr.service';
import { ToastrService } from 'ngx-toastr';
import { OttrMonitoring } from 'src/app/models/OttrMonitoring';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { CalenderComponent } from '../calender/calender.component';
import * as moment from 'moment';
import * as momentBusinessDays from '../../static_code/index';
import { TextareaComponent } from '../textarea/textarea.component';
import { ButtonRendererComponent } from 'src/app/renderer/button-renderer/button-renderer.component';
import { CustomHeaderComponent } from '../custom-header/custom-header.component';
import { forkJoin } from 'rxjs';
import { FileSaverService } from 'ngx-filesaver';
import { NgxSpinnerService } from "ngx-spinner";
import { _isNumberValue } from '@angular/cdk/coercion';
import { HeaderDeleteComponent } from '../header-delete/header-delete.component';

var regionData, gesGroupData, attributeToData, internalExternalAttributeToData, productData, gesGroupProductMappingData;
var surveyData = { NA: "NA", Required: "Required" }
var surveyTypes = { "Back Office": "Back Office", "Front Office": "Front Office" }
var surveyFor={Internal:"Internal",External:"External"}
var poleData, spendPlanRegionData;
var spendPlanCountryData;
var allSpendPlanRegionData;
var allgesGroupData;
@Component({
  selector: 'app-ottr-list',
  templateUrl: './ottr-list.component.html',
  styleUrls: ['./ottr-list.component.scss']
})
export class OttrListComponent implements OnInit, OnChanges {
   
  @Input() EditRole: boolean=false;
  @Input() GESGroupName: any;
  @Input() GESGroupId: any;
  filterParamData: any;

  setCheckbox(params: any) {
    var input = document.createElement('input');
    input.type = "checkbox";
    input.checked = params.value;
    input.style.position = "absolute";
    input.style.left = "20px";
    input.style.marginTop = "15px";
    var temp = this;
    input.addEventListener('click', function (event) {
      params.value = !params.value;
      params.node.data.delete = params.value;
      if (params.node.data.delete == true) {
        if (temp.ottrService.ottrDeleteId.filter(x => x == params.node.data.ottrId).length == 0) {
          temp.ottrService.ottrDeleteId.push(params.node.data.ottrId);
        }
      } else {
        temp.ottrService.ottrDeleteId = temp.ottrService.ottrDeleteId.filter(x => x != params.node.data.ottrId);
      }
     //////// console.log(temp.ottrService.ottrDeleteId);
    });
    if (params.node.data.ottrId != null && params.node.data.ottrId != '' && params.node.data.ottrId != undefined)
      return input;
    else
      return null;
  }
  // columnDefs is list of columns and its definitions

  columnDefs = [
    {
      headerComponent: 'headerDeleteComponent',
      field: 'delete',
      width: 50,
      lockVisible: true,
      lockPosition: true,
      cellRenderer: this.setCheckbox.bind(this),
      headerTooltip: 'Delete selected records',
    },
    {
      headerName: 'OTTR ID',
      field: 'ottrId',
      width: 120,
      lockVisible: true,
      lockPosition: true,
      sortable: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      headerTooltip: 'while filtering please click on text of AND and OR',
    },
    {
      headerName: 'GES PM / EM EID', field: 'gespmEID', width: 150,
      headerClass: 'mandatoryColumn',
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      lockPosition: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID to get PM Name"
      }
    },
    {
      headerName: 'GES PM / EM Name', field: 'gespm', width: 200,
      headerClass: 'mandatoryColumn',
      cellEditor: 'textareaComponent',
      editable: false,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      lockPosition: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID in GES PM/EM EID to get GES PM/EM Name"
      }
    },
    {
      headerName: 'Project ID', field: 'projectId', width: 150,
      headerComponent: 'customHeaderComponent',
      lockPosition: true,
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 50,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole

          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true
    },
    {
      headerName: 'Project Name', field: 'projectName', width: 200,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true
    },
    {
      headerName: 'Deliverables / Milestone', field: 'deliverables', width: 250,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 500,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: (params) =>
        params.data.deliverables
    },
    {
      headerName: 'Month', field: 'month', width: 100,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',      
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      valueGetter: this.getMonth,
    },
    {
      headerName: 'Base Date', field: 'baseDate', width: 160,
      headerClass: 'mandatoryColumn',
      lockPosition: true,
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      valueFormatter: this.formatDate
    },
    {
      headerName: 'Plan Date', field: 'planDate', width: 160,
      lockPosition: true,
      headerClass: "mandatoryColumn",
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      valueGetter: this.getDate,
      valueFormatter: this.formatDate
    },
    {
      headerName: 'Actual Date', field: 'actualDate', width: 160,
      lockPosition: true,
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      editable: true,
      cellEditorParams: (params) => {
        return {
          label: 'ActualDate',
          values: {
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      valueFormatter: this.formatDate
    },
    {
      headerName: 'Actual Date Modified On', field: 'actualDateModifiedOn', width: 160,
      lockPosition: true,
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      editable: false,
      valueFormatter: this.formatDate
    },
    {
      headerName: 'GES Group', field: 'gesGroupId', width: 100,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = gesGroupData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamsForGesDropDown.bind(this),
      editable: true,
      valueFormatter: this.getGESGroupDatabyGESGroupId
    },
    {
      headerName: 'GES Delivery Group', field: 'regionId', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = regionData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamsForRegionDropDown.bind(this),
      editable: true,
      valueFormatter: this.getRegionNameByRegionID
    },
    {
      headerName: 'Delivery Hours', field: 'deliveryHours', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 10,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole          
          }                    
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true
    },
    {
      headerName: 'Rework Hours', field: 'reworkHours', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        
        return {
          values: {
            maxlength: 10,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },          
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true

    },
    {
      headerName: 'Survey Request', field: 'surveyRequest', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = surveyData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamForServeyRequest.bind(this),
      editable: true,
      valueFormatter: this.getSurveyValue
    },
    {
      headerName: 'Pole', field: 'spendPlan_Pole', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = poleData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamForPole.bind(this),
      editable: true,
      valueFormatter: this.getPoleValue.bind(this)
    },
    {
      headerName: 'Region', field: 'spendPlan_Region', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = spendPlanRegionData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamForSpendPlanRegion.bind(this),
      editable: true,
      valueFormatter: this.getSpendPlanRegion
    },
    {
      headerName: 'Country', field: 'spendPlan_Country', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = spendPlanCountryData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamForSpendPlanCountry.bind(this),
      editable: true,
      valueFormatter: this.getSpendPlanCountry
    },
    {
      headerName: 'Survey Type', field: 'surveyType', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = surveyData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamForServeyType.bind(this),
      editable: true,
      valueFormatter: this.getSurveyTypeValue
    },
    {
      headerName: 'GES Resource ID', field: 'gesResourceId', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && params.data.surveyRequest.trim().indexOf("Required") > -1 && params.data.surveyType.trim().indexOf("Front Office") > -1 && this.EditRole,
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID to get GES Resource Name"
      }
    },
    {
      headerName: 'ges Resource Name', field: 'gesResourceName', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: false,
            role: this.EditRole
          }
        };
      },
      editable: false,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID in GES Resource ID to get GES Resource Name"
      }
    },
    {
      headerName: 'Product / Platform', field: 'productId', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = productData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      debounceMs: 2000,
      filter: true,
      cellEditorParams: this.setParamsForProductDropDown.bind(this),
      editable: true,
      valueFormatter: this.getProductNameByProductId
    },
    {
      headerName: 'Difference (Plan and Actual Date)',
      lockPosition: true,
      field: 'difference',
      width: 150,
      sortable: true,
      lockVisible: true,
      filter: false,
      valueGetter: this.calculateDifference,
      valueFormatter: this.calculateDifference
    },
    {
      headerName: 'Attribute to', field: 'attributeToId', width: 150,
      lockPosition: true,
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = attributeToData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParamsForAttributeDropDown.bind(this),
      editable: true,
      valueGetter: this.getDefaultAttribute,
      valueFormatter: this.getAttributeToDataByAttributeToId
    },


    {

      headerName: 'Tracking to Design Input', field: 'trackingToDesignInput', width: 250,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 500,
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },


      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: (params) =>
        params.data.trackingToDesignInput
    },

    {
      headerName: 'Internal / External Attribute', field: 'internalExternalAttributeId', width: 150,
      lockPosition: true,
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = internalExternalAttributeToData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      
      filter: true,
      cellEditorParams: this.setParamsForInternalExternalAttributeDropDown.bind(this),
      editable: true,
      valueGetter: this.getDefaultInternalExternalAttribute,
      valueFormatter: this.getAttributeToDataByInternalExternalAttributeId
    },

    {
      headerName: 'Comments(Change in Planned Date/ Delayed)', field: 'comments', width: 250,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 500,
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: (params) =>
        params.data.comments
    },
    {
      headerName: 'Survey For', field: 'surveyFor', width: 150,
     lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = surveyFor[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParaForServeyFor.bind(this),
      editable: true,
      valueFormatter: this.getSurveyForValue,
      onCellValueChanged: params => {
    this.onSurveyForChanged(params);
  }
    },
    {
      headerName: 'External Customer Name', field: 'exCustName', width: 200,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole && params.data.surveyFor !== 'Internal',
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid External Customer Name"
      }
    },
    {
      headerName: 'External customer email Address', field: 'exCustEmailId', width: 200,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole && params.data.surveyFor !== 'Internal',
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid External customer email Address"
      }
    },
    {
      headerName: 'FO PM EID (Multiple EID allowed with ; separator and with No space)', field: 'fopmId', width: 500,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            role: this.EditRole,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole && params.data.surveyFor !== 'External',
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        if (params.data.surveyFor === 'External') {     
           return "FOPM EID is disabled for External surveys";    
        }
        return "Enter Valid EID to get PM Name"
      }
    },
    {
      headerName: 'FO PM Name', field: 'fopmName', width: 500,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      autoHeight: true,
      cellStyle: {'white-space': 'normal'},
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      editable: false,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: true,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID in FO PM EID to get FO PM Name"
      }
    },

    {
      field: 'customerSurvey',
      hide: true
    },
    {
      field: 'customerSurveyTemplate',
      hide: true
    },
    {
      field: 'isActive',
      hide: true
    },
    {
      field: 'createdBy',
      hide: true
    },
    {
      field: 'createdOn',
      hide: true
    },
    {
      field: 'modifiedBy',
      hide: true
    },
    {
      field: 'modifiedOn',
      hide: true
    },
    {
      field: 'selectedRow',
      hide: true
    },

    {
      headerName: '', field: 'save', width: 0,
      lockPosition: true,
      cellRenderer: 'buttonRenderer',
      lockVisible: true,
      cellStyle: {
        'padding-left': '10px',
        'padding-right': '10px'
      },
      hide: !this.EditRole,
      pinned: 'right',
      suppressMovable: false,
      cellRendererParams: params => {
        return {
          onClick: this.createNewOttrData.bind(this),
          label: 'Save',
          isVisible: true,
          isDisabled: !this.modifiedRowIndex.includes(params.rowIndex)
        }
      }
    },
    {
      headerName: '', field: 'cancel', width: 0,
      lockPosition: true,
      suppressMovable: false,
      pinned: 'right',
      lockVisible: true,
      cellStyle: {
        'padding-left': '10px',
        'padding-right': '10px'
      },
      hide: !this.EditRole,
      cellRenderer: 'buttonRenderer',
      cellRendererParams: params => {
        return {
          onClick: this.cancelOttrData.bind(this),
          label: 'Cancel',
          isVisible: true,
          isDisabled: !this.modifiedRowIndex.includes(params.rowIndex)
        }
      }
    },
    {
      headerName: '', field: 'Copy', width: 0,
      lockPosition: true,
      cellRenderer: 'buttonRenderer',
      lockVisible: true,
      cellStyle: {
        'padding-left': '10px',
        'padding-right': '10px'
      },
      hide: !this.EditRole,
      pinned: 'right',
      suppressMovable: false,
      cellRendererParams: params => {
        return {
          onClick: this.copyNewRecord.bind(this),
          label: 'Copy',
          isVisible: true,
          isDisabled: this.copyFlag
        }
      }
    },
  ];
  columnDefswithoutFilter = [
    {
      headerName: 'OTTR ID',
      field: 'ottrId',
      width: 120,
      lockVisible: true,
      lockPosition: true,
      sortable: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      headerTooltip: 'while filtering please click on text of AND and OR',
    },
    {
      headerName: 'GES PM / EM EID', field: 'gespmEID', width: 150,
      headerClass: 'mandatoryColumn',
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      lockPosition: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID to get PM Name"
      }
    },
    {
      headerName: 'GES PM / EM Name', field: 'gespm', width: 200,
      headerClass: 'mandatoryColumn',
      cellEditor: 'textareaComponent',
      editable: false,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      lockPosition: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID in GES PM/EM EID to get GES PM/EM Name"
      }
    },
    {
      headerName: 'Project ID', field: 'projectId', width: 150,
      headerComponent: 'customHeaderComponent',
      lockPosition: true,
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 50,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole

          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false
    },
    {
      headerName: 'Project Name', field: 'projectName', width: 200,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false
    },
    {
      headerName: 'Deliverables / Milestone', field: 'deliverables', width: 250,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 500,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole,
            role: this.EditRole
          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: (params) =>
        params.data.deliverables
    },
    {
      headerName: 'Month', field: 'month', width: 100,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',      
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      valueGetter: this.getMonth,
    },
    {
      headerName: 'Base Date', field: 'baseDate', width: 160,
      headerClass: 'mandatoryColumn',
      lockPosition: true,
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      valueFormatter: this.formatDate
    },
    {
      headerName: 'Plan Date', field: 'planDate', width: 160,
      lockPosition: true,
      headerClass: "mandatoryColumn",
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      valueGetter: this.getDate,
      valueFormatter: this.formatDate
    },
    {
      headerName: 'Actual Date', field: 'actualDate', width: 160,
      lockPosition: true,
      cellEditor: 'calenderComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = moment(value).format("DD-MMM-YY");
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      editable: true,
      cellEditorParams: (params) => {
        return {
          label: 'ActualDate',
          values: {
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      valueFormatter: this.formatDate
    },
    {
      headerName: 'GES Group', field: 'gesGroupId', width: 100,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = gesGroupData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamsForGesDropDown.bind(this),
      editable: true,
      valueFormatter: this.getGESGroupDatabyGESGroupId
    },
    {
      headerName: 'GES Delivery Group', field: 'regionId', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = regionData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamsForRegionDropDown.bind(this),
      editable: true,
      valueFormatter: this.getRegionNameByRegionID
    },
    {
      headerName: 'Survey Request', field: 'surveyRequest', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = surveyData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamForServeyRequest.bind(this),
      editable: true,
      valueFormatter: this.getSurveyValue
    },
    {
      headerName: 'Pole', field: 'spendPlan_Pole', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = poleData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamForPole.bind(this),
      editable: true,
      valueFormatter: this.getPoleValue.bind(this)
    },
    {
      headerName: 'Region', field: 'spendPlan_Region', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = spendPlanRegionData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamForSpendPlanRegion.bind(this),
      editable: true,
      valueFormatter: this.getSpendPlanRegion
    },
    {
      headerName: 'Country', field: 'spendPlan_Country', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = spendPlanCountryData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamForSpendPlanCountry.bind(this),
      editable: true,
      valueFormatter: this.getSpendPlanCountry
    },
    {
      headerName: 'Survey Type', field: 'surveyType', width: 125,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = surveyData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamForServeyType.bind(this),
      editable: true,
      valueFormatter: this.getSurveyTypeValue
    },
    {
      headerName: 'GES Resource ID', field: 'gesResourceId', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && params.data.surveyRequest.trim().indexOf("Required") > -1 && params.data.surveyType.trim().indexOf("Front Office") > -1 && this.EditRole,
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID to get GES Resource Name"
      }
    },
    {
      headerName: 'ges Resource Name', field: 'gesResourceName', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: false,
            role: this.EditRole
          }
        };
      },
      editable: false,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID in GES Resource ID to get GES Resource Name"
      }
    },
    {
      headerName: 'Product / Platform', field: 'productId', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: true
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = productData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      debounceMs: 2000,
      filter: false,
      cellEditorParams: this.setParamsForProductDropDown.bind(this),
      editable: true,
      valueFormatter: this.getProductNameByProductId
    },
    {
      headerName: 'Difference (Plan and Actual Date)',
      lockPosition: true,
      field: 'difference',
      width: 150,
      sortable: true,
      lockVisible: true,
      filter: false,
      valueGetter: this.calculateDifference,
      valueFormatter: this.calculateDifference
    },
    {
      headerName: 'Attribute to', field: 'attributeToId', width: 150,
      lockPosition: true,
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = attributeToData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: false,
      cellEditorParams: this.setParamsForAttributeDropDown.bind(this),
      editable: true,
      valueGetter: this.getDefaultAttribute,
      valueFormatter: this.getAttributeToDataByAttributeToId
    },


    {

      headerName: 'Tracking to Design Input', field: 'trackingToDesignInput', width: 250,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 500,
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },


      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: (params) =>
        params.data.trackingToDesignInput
    },

    {
      headerName: 'Internal / External Attribute', field: 'internalExternalAttributeId', width: 150,
      lockPosition: true,
      cellEditor: 'dropdownComponent',
      sortable: true,
      lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = internalExternalAttributeToData[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      
      filter: false,
      cellEditorParams: this.setParamsForInternalExternalAttributeDropDown.bind(this),
      editable: true,
      valueGetter: this.getDefaultInternalExternalAttribute,
      valueFormatter: this.getAttributeToDataByInternalExternalAttributeId
    },

    {
      headerName: 'Comments(Change in Planned Date/ Delayed)', field: 'comments', width: 250,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      editable: true,
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 500,
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: (params) =>
        params.data.comments
    },
    {
      headerName: 'Survey For', field: 'surveyFor', width: 150,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      headerComponentParams: {
        isRequired: false
      },
      cellEditor: 'dropdownComponent',
      sortable: true,
      //lockVisible: true,
      filterParams: {
        textCustomComparator: function (_, value, filterText) {
          var filterTextLowerCase = filterText.toLowerCase();
          var valueLowerCase = surveyFor[value];
          if (valueLowerCase == undefined) {
            valueLowerCase = value;
          }
          valueLowerCase = valueLowerCase.toLowerCase();
          function contains(target, lookingFor) {
            if (_ == 'contains') {
              return target && target.indexOf(lookingFor) >= 0;
            }
            if (_ == 'notContains' || _ == 'notEqual') {
              return target && target.indexOf(lookingFor) == -1;
            }
            if (_ == 'equals') {
              return target && target == lookingFor;
            }
            if (_ == 'startsWith') {
              return target && target.startsWith(lookingFor);
            }
            if (_ == 'endsWith') {
              return target && target.endsWith(lookingFor);
            }
          }
          var literalMatch = contains(valueLowerCase, filterTextLowerCase);
          return (
            literalMatch
          );
        },
         
        newRowsAction:'keep',
        debounceMs: 2000
      },
      filter: true,
      cellEditorParams: this.setParaForServeyFor.bind(this),
      editable: true,
      valueFormatter: this.getSurveyForValue,
      onCellValueChanged: params => {
    this.onSurveyForChanged(params);
  }
    },
    {
       headerName: 'External Customer Name', field: 'exCustName', width: 200,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole && params.data.surveyFor !== 'Internal',
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid External Customer Name"
      }
    },
    {
      headerName: 'External customer email Address', field: 'exCustEmailId', width: 200,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole && params.data.surveyFor !== 'Internal',
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID to get PM Name"
      }
    },
    {
      headerName: 'FO PM EID (Multiple EID allowed with ; separator and with No space)', field: 'fopmId', width: 500,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 150,
            isEditable: (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && this.EditRole && params.data.surveyFor !== 'External',
            role: this.EditRole
          }
        };
      },
      editable: true,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID to get PM Name"
      }
    },
    {
      headerName: 'FO PM Name', field: 'fopmName', width: 500,
      lockPosition: true,
      headerComponent: 'customHeaderComponent',
      cellEditor: 'textareaComponent',
      autoHeight: true,
      cellStyle: {'white-space': 'normal'},
      cellEditorParams: (params) => {
        return {
          values: {
            maxlength: 100,
            isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
            role: this.EditRole
          }
        };
      },
      editable: false,
      sortable: true,
      lockVisible: true,
      filterParams: {
        newRowsAction: 'keep'
      },
      filter: false,
      tooltip: function (params) {//This will show valueFormatted if is present, if no just show the value.
        return "Enter Valid EID in FO PM EID to get FO PM Name"
      }
    },

    {
      field: 'customerSurvey',
      hide: true
    },
    {
      field: 'customerSurveyTemplate',
      hide: true
    },
    {
      field: 'isActive',
      hide: true
    },
    {
      field: 'createdBy',
      hide: true
    },
    {
      field: 'createdOn',
      hide: true
    },
    {
      field: 'modifiedBy',
      hide: true
    },
    {
      field: 'modifiedOn',
      hide: true
    },
    {
      field: 'selectedRow',
      hide: true
    },

    {
      headerName: '', field: 'save', width: 0,
      lockPosition: true,
      cellRenderer: 'buttonRenderer',
      lockVisible: true,
      cellStyle: {
        'padding-left': '10px',
        'padding-right': '10px'
      },
      hide: !this.EditRole,
      pinned: 'right',
      suppressMovable: false,
      cellRendererParams: params => {
        return {
          onClick: this.createNewOttrData.bind(this),
          label: 'Save',
          isVisible: true,
          isDisabled: !this.modifiedRowIndex.includes(params.rowIndex)
        }
      }
    },
    {
      headerName: '', field: 'cancel', width: 0,
      lockPosition: true,
      suppressMovable: false,
      pinned: 'right',
      lockVisible: true,
      cellStyle: {
        'padding-left': '10px',
        'padding-right': '10px'
      },
      hide: !this.EditRole,
      cellRenderer: 'buttonRenderer',
      cellRendererParams: params => {
        return {
          onClick: this.cancelOttrData.bind(this),
          label: 'Cancel',
          isVisible: true,
          isDisabled: !this.modifiedRowIndex.includes(params.rowIndex)
        }
      }
    },
    {
      headerName: '', field: 'Copy', width: 0,
      lockPosition: true,
      cellRenderer: 'buttonRenderer',
      lockVisible: true,
      cellStyle: {
        'padding-left': '10px',
        'padding-right': '10px'
      },
      hide: !this.EditRole,
      pinned: 'right',
      suppressMovable: false,
      cellRendererParams: params => {
        return {
          onClick: this.copyNewRecord.bind(this),
          label: 'Copy',
          isVisible: true,
          isDisabled: this.copyFlag
        }
      }
    },
  ];
  
  rowData = [];
  modifiedRowIndex = [];
  private gridApi;
  private gridColumnApi;
  rowClassRules: any;
  frameworkComponents;
  addInProgress = false;
  attributeToFlag = false;
  ottrMonitoringData = null;
  defaultColDef = {
    resizable: false,

  };
  @Input() userID: string;
  @Input() addRequest: string;
  @Output() disableCreateButton = new EventEmitter();
  @Output() disableFilter = new EventEmitter();
  @Output() gridPopulateEvent = new EventEmitter();
  selectedGESGroup = -1;
  GESGroups = [];
  rowDataCopy: any;
  baseDateCommentFlag: boolean;
  copyFlag: boolean;
  planDateCommentFlag: boolean;
  selectOptionSelected = false;
  filterModel: string;

  constructor(private ottrService: OttrService, private toastr: ToastrService, private _FileSaverService: FileSaverService, private spinner: NgxSpinnerService) {
    this.rowClassRules = {
      'Row-Highliter': function (params) {
        if (params.data.selectedRow === true) {
          return true;
        }
        return false;
      },

    };
    momentBusinessDays.updateLocale('us', {
      workingWeekdays: [1, 2, 3, 4, 5]
    });
    this.frameworkComponents = {
      textareaComponent: TextareaComponent,
      dropdownComponent: DropdownComponent,
      calenderComponent: CalenderComponent,
      buttonRenderer: ButtonRendererComponent,
      customHeaderComponent: CustomHeaderComponent,
      headerDeleteComponent: HeaderDeleteComponent
    };    
  }

  ngOnChanges(changes: import("@angular/core").SimpleChanges): void {
    if (changes.addRequest != undefined) {
      if (changes.addRequest.currentValue != changes.addRequest.previousValue) {
        if (this.selectedGESGroup == -1) {
          this.GESGroupName = "All";
          this.selectOptionSelected = true;
          this.getGESSubgroups();
          this.spinner.show();
        }
        else {
          this.addNewRecord();
        }
      }
    }


  }
onSurveyForChanged(params: any) {
  const value = params.data.surveyFor;

  if (value === 'External') {
    params.data.fopmId = null;
    params.data.fopmName = null;
    params.data.disableFopm = true;
  } else {
    params.data.disableFopm = false;
  }
  if (value === 'Internal') {
    params.data.exCustName = null;
    params.data.exCustEmailId = null;
  }
  // Update the grid so editable and styles re-evaluate
  this.gridApi.refreshCells({ force: true });
}





  getGESSubgroupsForSelectOption() {
    this.ottrService.getGESGroupData(this.GESGroupName).subscribe(data => {
      gesGroupData = data;
      this.addNewRecord();
    }, err => {
      this.toastr.error('Failed to load data from server!', 'Error');
      console.error(err);
    });
  }

  ngOnInit(): void {
    this.ottrService.gesGroupChangeEvent.subscribe(data => {
      this.onSelectGESGroup(data);
    });
    this.ottrService.uploadExcelEvent.subscribe(data => {     
      this.getOTTRList();
    });
    this.ottrService.filterClearEvent.subscribe(data=>{
      if(data == true) {
        this.gridApi.setFilterModel(null);       
        this.disableCreateButton.emit(false);
      }
    });
    this.ottrService.surveyRequestChangeEvent.subscribe(data => {
      if (data != null) {
        var index = this.rowData.findIndex(x => x.ottrId == data.data.ottrId);
        if (data.value.indexOf("NA") > -1) {
          this.columnDefs[12].editable = false;
          this.columnDefs[13].editable = false;
          this.columnDefs[14].editable = false;
        }
        else {
          this.columnDefs[12].editable = true;
          this.columnDefs[13].editable = true;
          this.columnDefs[14].editable = true;
        }
      }
    });
    this.getAllSubgroupData();
  }

  //method for all selection
  getAllSubgroupData() {
    this.ottrService.getAllGESGroupData().subscribe(data => {
      allgesGroupData = data;
    }, err => { });
  }


  //function to retrieve ottr monitoring data via http interface
  getOTTRList() {
    if (this.gridColumnApi != undefined) {
      this.gridColumnApi.setColumnsVisible(['save', 'cancel', 'Copy'], this.EditRole)
    }
    this.EditRole;
    this.ottrMonitoringData = null;
    this.planDateCommentFlag = this.baseDateCommentFlag = false;
    this.copyFlag = !this.EditRole;
    this.rowData = [];
    this.spinner.show();
    if (this.selectedGESGroup != -1 && this.selectedGESGroup != undefined) {
      this.ottrService.getOTTRMonitoringData(this.selectedGESGroup).subscribe((data: any) => {
        if (data == undefined) {
          this.spinner.hide();
          return;
        }
        if (data.Status == 401 && data.data == "unauthorized access") {
          window.location.href = data.URL;
        } else {
          this.filterParamData = {};
         // this.gridApi.setFilterModel(null);
          this.rowData = [];
          data.forEach(element => {            
            if (element.surveyRequest == null || element.surveyRequest == "" || element.surveyRequest == undefined || element.surveyRequest.trim() != "Required")
              element.subgroupData = {};
            var obj: any = {};
            for (var item of allgesGroupData) {
              var matchfound = false;
              var filteredgroup = item.gesSubGroup.filter(x => x.subgroupId == element.gesGroupId);
              if (filteredgroup.length > 0) {
                obj[filteredgroup[0].subgroupId] = filteredgroup[0].subgroupName;
                matchfound = true;
              }
              if (matchfound == true) {
                var othersubgroups = item.gesSubGroup.filter(x => x.subgroupId != element.gesGroupId);
                for (var subgroup of othersubgroups) {
                  obj[subgroup.subgroupId] = subgroup.subgroupName;
                }
              }
            }
            element.subgroupData = obj;
            element.delete = false;
          });
          this.rowData = data;
          if (this.rowData.length === 0) {
            this.gridPopulateEvent.emit(true);
          } else {
            this.gridPopulateEvent.emit(false);
          }
          this.modifiedRowIndex = [];
         //////// console.log("inside get ottr list");
          if (this.ottrService!=undefined && this.ottrService.roleList!=undefined && this.ottrService.roleList.filter(x => x == "OTTR-Admin").length > 0) {
            this.gridColumnApi.setColumnVisible('delete', true);
          }
        }
        this.disableCreateButton.emit(!this.EditRole);
        var temp = this.gridApi.getFilterModel();
        var temp2 = JSON.stringify(temp);
        if (temp2.trim() !== "{}") {
          this.disableCreateButton.emit(true);
        }
        else {
          this.disableCreateButton.emit(false);
        }
        setTimeout(()=>{this.spinner.hide();},2000);
      }, err => {
        this.spinner.hide();
        console.error(err);
        this.toastr.error('Failed to load data due to server error!', 'Error');
      });
    } else {
      this.spinner.hide();
      this.rowData = [];
      this.modifiedRowIndex = [];
      if (this.selectOptionSelected == true) {
        this.selectOptionSelected = false;
        this.addNewRecord();
      }
    }
  }

  onSelectGESGroup(selectedGESGroup) {
    this.selectedGESGroup = selectedGESGroup;
    //temporarily assigned id = -1 for select GES group option in dropdown
    if (selectedGESGroup != -1) {
      this.getGESSubgroups();
    } else {
      this.rowData = [];
      this.gridPopulateEvent.emit(true);
      this.modifiedRowIndex = [];
    }
  }

  getSpendPlanRegionData(poleId: any) {
    spendPlanRegionData = null;
    this.ottrService.getSpendPlanRegionData(poleId).subscribe(data => {
      spendPlanRegionData = data;
    }, err => {
      this.toastr.error('Failed to load data from server!', 'Error');
      console.error(err);
    });
  }

  getSpendPlanCountryData(regionId: any) {
    spendPlanCountryData = null;
    this.ottrService.getSpendPlanCountryData(regionId).subscribe(data => {
      spendPlanCountryData = data;
    }, err => {
      this.toastr.error('Failed to load data from server!', 'Error');
      console.error(err);
    });
  }

  getGESSubgroups() {
    this.ottrService.getGESGroupData(this.GESGroupName).subscribe(data => {
      gesGroupData = data;
      this.loadData();
    }, err => {
      this.toastr.error('Failed to load data from server!', 'Error');
      console.error(err);
    });
  }


  //function to retrieve all data from all Master Tables and Mapping Tables
  async loadData() {
    forkJoin(this.ottrService.getRegionData(),
      this.ottrService.getAttributeToData(), this.ottrService.getProductData(),
      this.ottrService.getGESGroupProductMappingData(),
      this.ottrService.getInternalExternalAttributeToData(),
      this.ottrService.getPoleData(),
      this.ottrService.getAllSpendPlanRegions(),
      this.ottrService.getAllSpendPlanCountryData()
    ).subscribe((data: any) => {
      // //console.log(("data", data);
      if (data.Status == 401 && data.data == "unauthorized access") {
        window.location.href = data.URL;
      } else {
        regionData = data[0];
        attributeToData = data[1];
        // //console.log((attributeToData);
        productData = data[2];
        gesGroupProductMappingData = data[3];
        internalExternalAttributeToData = data[4];
        poleData = data[5];
        spendPlanRegionData = data[6];
        spendPlanCountryData = data[7];
        this.getOTTRList();
      }
    }, err => {
      this.toastr.error('Failed to load data from server!', 'Error');
      console.error(err);
    });
  }




  getFilterParams() {
    var temp = this.gridApi.getFilterModel();
    var temp2 = JSON.stringify(temp);
    this.filterModel = temp2;
    var temp3 = temp2;
    if (temp3.indexOf("ottrId") > -1) {
      temp3 = temp3.replace("ottrId", "OTTR_ID");
    }
    if (temp3.indexOf("gespmEID") > -1) {
      temp3 = temp3.replace("gespmEID", "GES_PM_EID");
    }
    if (temp3.indexOf("gespm") > -1) {
      temp3 = temp3.replace("gespm", "GES_PM");
    }
    if (temp3.indexOf("surveyRequest") > -1) {
      temp3 = temp3.replace("surveyRequest", "Survey_Request");
    } 
    if(temp3.indexOf("surveyFor")>-1)
    {
      temp3=temp3.replace("surveyFor","survey_For")
    }
    if (temp3.indexOf("fopmName") > -1) {
      temp3 = temp3.replace("fopmName", "FO_PM_Name");
    }
    if (temp3.indexOf("fopmId") > -1) {
      temp3 = temp3.replace("fopmId", "FO_PM_ID");
    }
    if (temp3.indexOf("gesGroupId") > -1) {
      temp3 = temp3.replace("gesGroupId", "GES_Sub_Group");
    }
    if (temp3.indexOf("spendPlan_Region") > -1) {
      temp3 = temp3.replace("spendPlan_Region", "SPregionName");
    }
    if (temp3.indexOf("spendPlan_Pole") > -1) {
      temp3 = temp3.replace("spendPlan_Pole", "PoleName");
    }
    if (temp3.indexOf("spendPlan_Country") > -1) {
      temp3 = temp3.replace("spendPlan_Country", "CountryName");
    }
    if (temp3.indexOf("productId") > -1) {
      temp3 = temp3.replace("productId", "ProductName");
    }
    if (temp3.indexOf("attributeToId") > -1) {
      temp3 = temp3.replace("attributeToId", "AttributeTo");
    }
    if (temp3.indexOf("internalExternalAttributeId") > -1) {
      temp3 = temp3.replace("internalExternalAttributeId", "InternalExternalAttributeTo");
    }
    if (temp3.indexOf("spendPlan_Country") > -1) {
      temp3 = temp3.replace("spendPlan_Country", "CountryName");
    }
    if (temp3.indexOf("regionId") > -1) {
      temp3 = temp3.replace("regionId", "RegionName");
    }
    if (temp3.indexOf("surveyType") > -1) {
      temp3 = temp3.replace("surveyType", "Survey_Type");
    }
    if (temp3.indexOf("gesResourceId") > -1) {
      temp3 = temp3.replace("gesResourceId", "GES_Resource_EID");
    }
    if (temp3.indexOf("gesResourceName") > -1) {
      temp3 = temp3.replace("gesResourceName", "GES_Resource_Name");
    }
    if(temp3.indexOf("exCustEmailId")>-1){
      temp3=temp3.replace("exCustEmailId","exCust_EmailId");
    }
    if(temp3.indexOf("exCustName")>-1){
      temp3=temp3.replace("exCustName","exCust_Name");
    }
    temp3 = JSON.parse(temp3);
    return temp3;
  }


  //function to handle grid ready event
  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.spinner.show();
    this.ottrService.exportData.subscribe(data => {
      if (data && this.selectedGESGroup != -1 && this.selectedGESGroup != undefined) {
        var filterParams = null;
        setTimeout(() => {
          filterParams = this.getFilterParams();
          this.ottrService.downloadOttrData(this.selectedGESGroup, filterParams).subscribe(res => {
            //console.log(res.body);
            let blob = new Blob([res.body], {
              type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-16le"
            });
            if (blob.size < 1000) {
              var obj: any = {};
              obj.data = "Unauthorized Access";
              obj.code = 401;
              this.ottrService.triggerUnauthorizedAccessEvent(obj);
              this.spinner.hide();
              return;
            }
            if (res != undefined) {
              this._FileSaverService.save(res.body, 'OttrData.xlsx');
            }
            this.spinner.hide();
          }, err => {
            this.spinner.hide();
          });
        }, 200);



      }
    });
    //if (this.ottrService?.roleList?.filter(x => x == "OTTR-Admin").length == 0) {
    this.gridColumnApi.setColumnVisible('delete', false);
    //}
  }

  //function to set list of regions to dropDown Component
  setParamsForRegionDropDown(params) {
    return {
      values: {
        dropDownValues: regionData,
        isEditable: this.EditRole ? ((this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex))) : false,
        role: this.EditRole
      }
    };
  }
  setParamForServeyRequest(params) {
    return {
      values: {
        dropDownValues: surveyData,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
        role: this.EditRole
      }
    };
  }

setParaForServeyFor(params) {
  return {
      values: {
        dropDownValues: surveyFor,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) :false,
        role: this.EditRole
      }
    };
}

  setParamForServeyType(params) {
    return {
      values: {
        dropDownValues: surveyTypes,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && params.data.surveyRequest.trim().indexOf("Required") > -1 : false,
        role: this.EditRole
      }
    };
  }
  setParamForPole(params) {
    return {
      values: {
        dropDownValues: poleData,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && params.data.surveyRequest.trim().indexOf("Required") > -1 : false,
        role: this.EditRole
      }
    };
  }
  setParamForSpendPlanRegion(params) {
    return {
      values: {
        dropDownValues: spendPlanRegionData,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && params.data.surveyRequest.trim().indexOf("Required") > -1 : false,
        role: this.EditRole
      }
    };
  }

  setParamForSpendPlanCountry(params) {
    return {
      values: {
        dropDownValues: spendPlanCountryData,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) && params.data.surveyRequest.trim().indexOf("Required") > -1 : false,
        role: this.EditRole
      }
    };
  }
  //set ges dropdown values
  setParamsForGesDropDown(params) {
    return {
      values: {
        dropDownValues: gesGroupData,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
        role: this.EditRole
      }
    };
  }

  setParamsForInternalExternalAttributeDropDown(params) {
    return {
      values: {
        dropDownValues: internalExternalAttributeToData,
        isEditable: this.EditRole ? ((this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex))) : false,
        role: this.EditRole
      }
    };
  }

  //set attributes list to dropdown
  setParamsForAttributeDropDown(params) {
    const a = momentBusinessDays(params.data.planDate);
    const b = momentBusinessDays(params.data.actualDate);
    const dayDifference = a.businessDiff(b, true);
    const data = { ...attributeToData };
    var dropDownValues = {};
    delete data[4];
    data[0] = null;

    if (params.data.actualDate) {
      dropDownValues = dayDifference >= 0 ? { 4: "On Time" } : data;
      if (dayDifference >= 0) {
        this.attributeToFlag = true;
      }
    }
    else {
      dropDownValues = data;
      this.attributeToFlag = false;

    }
    return {
      values: {
        dropDownValues,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
        role: this.EditRole
      }
    };
  }

  //set list of products to dropdown
  setParamsForProductDropDown(params) {
    const gesGroupId = this.getGESGroupIdByGESGroupName(params.data.gesGroupId);
    const listOfProductIds = gesGroupProductMappingData[gesGroupId];
    if (listOfProductIds === undefined) {
      return {
        values: {
          dropDownValues: productData,
          isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
          role: this.EditRole
        }
      };
    }
    const customProductData = {};
    listOfProductIds.forEach(element => {
      customProductData[element] = productData[element];
    });
    return {
      values: {
        dropDownValues: customProductData,
        isEditable: this.EditRole ? (this.modifiedRowIndex.length === 0 || this.modifiedRowIndex.includes(params.rowIndex)) : false,
        role: this.EditRole
      }
    };
  }

  //function to show Region Names corresponding to Region Ids
  getRegionNameByRegionID(params) {
    return regionData[params.value];
  }

  getSurveyValue(params) {
    return surveyData[params.value];
  }

  getSurveyForValue(params){
    return surveyFor[params.value];
  }
  getSurveyTypeValue(params) {
    return surveyTypes[params.value];
  }

  getPoleValue(params) {
    if (poleData != null && poleData != undefined)
      return poleData[params.value];
  }

  getSpendPlanCountry(params) {
    if (spendPlanCountryData != null && spendPlanCountryData != undefined) {
      return spendPlanCountryData[params.value];
    }
  }

  getSpendPlanRegion(params) {
    if (spendPlanRegionData != null && spendPlanRegionData != undefined) {
      return spendPlanRegionData[params.value];
    }
  }
  //function to show GESGroup Data corresponding to GESGroup Ids
  getGESGroupDatabyGESGroupId(params) {
    if (params.value == -1 || params.value == 0) {
      return '';
    }
    return gesGroupData[params.value];
  }


  //function to show AttributeTo Data corresponding to AttributeTo Ids
  getAttributeToDataByAttributeToId(params) {
    return !(params.value) ? '' : attributeToData[params.value];
  }
  getAttributeToDataByInternalExternalAttributeId(params) {
    return !(params.value) ? '' : internalExternalAttributeToData[params.value];
  }

  //function to show Product Name corresponding to Product Ids
  getProductNameByProductId(params) {
    return productData[params.value];
  }

  //Get Region Id by Region Name
  getRegionIdByRegionName(regionName) {
    for (const region in regionData) {
      if (regionData[region] === regionName) {
        return Number(region);
      }
    }
    return regionName;
  }

  //Get GESGroup Id by GESGRoup Name
  getGESGroupIdByGESGroupName(gesGroupName) {
    for (const gesGroup in gesGroupData) {
      if (gesGroupData[gesGroup] === gesGroupName) {
        return Number(gesGroup);
      }
    }
    return Number(gesGroupName);
  }

  //Get AttributeTo Id by AttributeToName
  getAttributeToIdByAttributeToName(attributeToName) {
    for (const attributeTo in attributeToData) {
      if (attributeToData[attributeTo] === attributeToName) {
        return Number(attributeTo);
      }
    }
    return attributeToName;
  }
  //Get AttributeTo Id by AttributeToName
  getInternalExternalAttributeIdByAttributeToName(attributeToName) {
    for (const attributeTo in internalExternalAttributeToData) {
      if (internalExternalAttributeToData[attributeTo] === attributeToName) {
        return Number(attributeTo);
      }
    }
    return attributeToName;
  }
  //Get Product Id by Product Name
  getProductIdByProductName(productName) {
    for (const product in productData) {
      if (productData[product] === productName) {
        return Number(product);
      }
    }
    return productName;
  }

  // formats date to desired format
  formatDate(params) {
    return params.value === null || params.value === '' ? null : moment(params.value).format("DD-MMM-YY");
  }

  //get default value of attributeto
  getDefaultAttribute(params) {
    if (!params.data.planDate || !params.data.actualDate) {
      if (params.data.attributeToId) {
        return params.data.attributeToId;
      }
      if (!params.data.actualDate) {
        return 0;
      }
      return null;
    }



    if (!params.data.actualDate) {
      if (params.data.attributeToId) {
        return params.data.attributeToId;
      }

      return 3;
    }
    const a = momentBusinessDays(params.data.planDate);
    const b = momentBusinessDays(params.data.actualDate);
    var dayDifference = a.businessDiff(b, true);
    if (dayDifference >= 0) {

      return 4; //place id of object with value "On Time"
    }
    if (params.data.attributeToId) {
      return params.data.attributeToId;
    }

  }

  //get default value of attributeto
  getDefaultInternalExternalAttribute(params) {
    if (params.data.internalExternalAttributeId) {
      return params.data.internalExternalAttributeId;
    }
    return null;
  }


  //Calculate the Difference between Plan Date and Actual Date
  calculateDifference(params) {
    if (!(params.data.planDate) || !(params.data.actualDate)) {
      return null;
    }
    const a = momentBusinessDays(params.data.planDate);
    const b = momentBusinessDays(params.data.actualDate);
    return a.businessDiff(b, true);
  }

  getMonth(params) {
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    if (params.data.planDate) {
      return (monthNames[moment(params.data.planDate).toDate().getMonth()] + '-' + moment(params.data.planDate).toDate().getFullYear());
    }
  }


  calculateMonth(params) {
    const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    if (params.planDate) {
      return (monthNames[moment(params.planDate).toDate().getMonth()] + '-' + moment(params.planDate).toDate().getFullYear());
    }
  }

  getDate(params) {
    if (params.data.planDate != null && params.data.planDate != undefined) {
      return params.data.planDate;
    }
    else {
      return null;
    }
  }






  onCellValueChanged(event) {
    if (this.EditRole) {
      this.copyFlag = true;
      if (this.ottrMonitoringData === null) {
        this.ottrMonitoringData = { ...event.data };
        this.ottrMonitoringData[event.colDef.field] = event.oldValue;
      }
      if (event.colDef.field == "gespmEID") {
        for (var rowItem of this.rowData) {
          if (rowItem.ottrId == event.data.ottrId && (event.data.gespmEID == null || event.data.gespmEID == "" || event.data.gespmEID == undefined)) {
            rowItem.gespm = null;
            this.gridApi.redrawRows();
            break;
          }
        }
      }
      if (event.colDef.field == "gesResourceId") {
        for (var rowItem of this.rowData) {
          if (rowItem.ottrId == event.data.ottrId && (event.data.gesResourceId == null || event.data.gesResourceId == "" || event.data.gesResourceId == undefined)) {
            rowItem.gesResourceName = null;
            this.gridApi.redrawRows();
            break;
          }
        }
      }

      if (event.colDef.field == "fopmId") {
        for (var rowItem of this.rowData) {
          if (rowItem.ottrId == event.data.ottrId && (event.data.fopmId == null || event.data.fopmId == "" || event.data.fopmId == undefined)) {
            rowItem.fopmId = null;
            rowItem.fopmName = null;
            this.gridApi.redrawRows();
            break;
          }
        }
      }
      if (event.colDef.field == "baseDate" || event.colDef.field == "planDate") {
        if (event.colDef.field == "baseDate" && (event.newValue != event.oldValue)) {
          this.baseDateCommentFlag = true;
        }
        else {
          this.baseDateCommentFlag = false;
        }
        if (event.colDef.field == "planDate" && (event.newValue != event.oldValue)) {
          this.planDateCommentFlag = true;
        }
        else {
          this.planDateCommentFlag = false;
        }
      }



      if (this.modifiedRowIndex.length === 0 && !this.modifiedRowIndex.includes(event.rowIndex)) {
        event.data.selectedRow = true;
        this.disableCreateButton.emit(true);
        this.modifiedRowIndex.push(event.rowIndex);
        this.gridApi.redrawRows();
      }

      if (event.data != null && !event.data.planDate) {
      }

    }
  }
  onSaveSuccess(rowIndex, rowNode) {
    const index = this.modifiedRowIndex.indexOf(rowIndex);
    if (index > -1) {
      this.modifiedRowIndex.splice(index, 1);
      this.gridApi.redrawRows({ rowNodes: [rowNode] });
    }
    this.ottrMonitoringData = null;
  }

  //function to add new row in table
  addNewRecord() {
    this.copyFlag = true;
    this.disableCreateButton.emit(true);
    this.addInProgress = true;
    var newItem = new OttrMonitoring();
    newItem.gesGroupId = this.selectedGESGroup;
    newItem.surveyRequest = "NA";
    newItem.selectedRow = true;
    newItem.surveyFor="Internal";
    this.rowData.unshift(newItem);
    this.modifiedRowIndex.push(0);
    this.gridApi.updateRowData({ addIndex: 0, add: [newItem] });
    this.gridApi.setFocusedCell(0, 'gespm');
    this.gridApi.redrawRows();

    this.gridApi.startEditingCell({
      rowIndex: 0,
      colKey: 'gespm'
    });
  }
  //function to add new row in table copy from  existing
  copyNewRecord(e) {
    this.copyFlag = true;
    this.disableCreateButton.emit(true);
    this.addInProgress = true;
    var newItem = new OttrMonitoring();
    var copyottr = new OttrMonitoring();
    copyottr = { ...e.rowNode.data };
    newItem.spendPlan_Pole = copyottr.spendPlan_Pole;
    newItem.spendPlan_Region = copyottr.spendPlan_Region;
    newItem.spendPlan_Country = copyottr.spendPlan_Country;
    newItem.productId = copyottr.productId;
    newItem.projectId = copyottr.projectId;
    newItem.projectName = copyottr.projectName;
    newItem.fopmId = copyottr.fopmId;
    newItem.attributeToId = copyottr.attributeToId;
    newItem.fopmName = copyottr.fopmName;
    newItem.gesGroupId = copyottr.gesGroupId;
    newItem.gespm = copyottr.gespm;
    newItem.gespmEID = copyottr.gespmEID;
    newItem.baseDate = copyottr.baseDate;
    newItem.planDate = copyottr.planDate;
    newItem.actualDate = copyottr.actualDate;
    newItem.regionId = copyottr.regionId;
    newItem.comments = copyottr.comments;
    newItem.deliveryHours = copyottr.deliveryHours;
    newItem.reworkHours = copyottr.reworkHours;
    newItem.surveyRequest = copyottr.surveyRequest;
    newItem.deliverables = copyottr.deliverables;
    newItem.surveyType = copyottr.surveyType;
    newItem.gesResourceId = copyottr.gesResourceId;
    newItem.gesResourceName = copyottr.gesResourceName;
    newItem.internalExternalAttributeId = copyottr.internalExternalAttributeId;
    newItem.trackingToDesignInput = copyottr.trackingToDesignInput;
    newItem.selectedRow = true;
    newItem.surveyFor=copyottr.surveyFor;
    newItem.exCustEmailId=copyottr.exCustEmailId;
    newItem.exCustName=copyottr.exCustName;
    this.rowData.unshift(newItem);
    this.gridApi.updateRowData({ addIndex: 0, add: [newItem] });
    this.gridApi.setFocusedCell(0, 'gespm');

    this.gridApi.redrawRows();
    this.modifiedRowIndex.push(0);
    this.gridApi.startEditingCell({
      rowIndex: 0,
      colKey: 'gespm'
    });
  }


  getKeyFromValue(object, value) {
    for (var key in object) {
      if (object[key] === value) {
        return key;
      }
    }
  }


  handleFilterEvent(e) {

    // //console.log((e);
    var temp = this.gridApi.getFilterModel();
    // //console.log((temp)
    var temp2 = JSON.stringify(temp);
    this.filterModel = temp2;
    // if(this.addInProgress == true)
    // {
    //   if(temp2.trim() !== "{}")
    //   {
    //     this.toastr.error("Filters cannot be used while creating new OTTR","Error");
    //     this.gridApi.setFilterModel(null);        
    //   }
    // }
    if (temp2.trim() === "{}") {
      this.disableCreateButton.emit(false);
      this.disableFilter.emit(true);
    }
    else {
      this.disableFilter.emit(false);
      this.disableCreateButton.emit(true);
    }
    var temp3 = temp2;
    if (temp3.indexOf("ottrId") > -1) {
      temp3 = temp3.replace("ottrId", "OTTR_ID");
    }
    if (temp3.indexOf("gespmEID") > -1) {
      temp3 = temp3.replace("gespmEID", "GES_PM_EID");
    }
    if (temp3.indexOf("gespm") > -1) {
      temp3 = temp3.replace("gespm", "GES_PM");
    }
    if (temp3.indexOf("surveyRequest") > -1) {
      temp3 = temp3.replace("surveyRequest", "Survey_Request");
    }
    if (temp3.indexOf("fopmName") > -1) {
      temp3 = temp3.replace("fopmName", "FO_PM_Name");
    }
    if (temp3.indexOf("fopmId") > -1) {
      temp3 = temp3.replace("fopmId", "FO_PM_ID");
    }
    if (temp3.indexOf("gesGroupId") > -1) {
      temp3 = temp3.replace("gesGroupId", "GES_Sub_Group");
    }
    if (temp3.indexOf("spendPlan_Region") > -1) {
      temp3 = temp3.replace("spendPlan_Region", "SPregionName");
    }
    if (temp3.indexOf("spendPlan_Pole") > -1) {
      temp3 = temp3.replace("spendPlan_Pole", "PoleName");
    }
    if (temp3.indexOf("spendPlan_Country") > -1) {
      temp3 = temp3.replace("spendPlan_Country", "CountryName");
    }
    if (temp3.indexOf("productId") > -1) {
      temp3 = temp3.replace("productId", "ProductName");
    }
    if (temp3.indexOf("attributeToId") > -1) {
      temp3 = temp3.replace("attributeToId", "AttributeTo");
    }
    if (temp3.indexOf("internalExternalAttributeId") > -1) {
      temp3 = temp3.replace("internalExternalAttributeId", "InternalExternalAttributeTo");
    }
    if (temp3.indexOf("spendPlan_Country") > -1) {
      temp3 = temp3.replace("spendPlan_Country", "CountryName");
    }
    if (temp3.indexOf("regionId") > -1) {
      temp3 = temp3.replace("regionId", "RegionName");
    }
    if (temp3.indexOf("surveyType") > -1) {
      temp3 = temp3.replace("surveyType", "Survey_Type");
    }
    if (temp3.indexOf("gesResourceId") > -1) {
      temp3 = temp3.replace("gesResourceId", "GES_Resource_EID");
    }
    if (temp3.indexOf("gesResourceName") > -1) {
      temp3 = temp3.replace("gesResourceName", "GES_Resource_Name");
    }

    if(temp3.indexOf("surveyFor")>-1){
      temp3=temp3.replace("surveyFor","survey_For")
    }
    if(temp3.indexOf("exCustEmailId")>-1){
      temp3=temp3.replace("exCustEmailId","exCust_EmailId");
    }
    if(temp3.indexOf("exCustName")>-1){
      temp3=temp3.replace("exCustName","exCust_Name");
    }
    temp3 = JSON.parse(temp3);
    // //console.log((temp3);
    this.filterParamData = temp3;
  }

  handleSortEvent(e) {
    var temp = this.gridApi.getSortModel();
    // //console.log((temp);
  }

  //creates ottr data and sends it to DB

  validateFOPMName(ottrData, rowIndex) {
    var validationResult = true;
    if (ottrData.fopmId == null || ottrData.fopmId == '') {
      validationResult = true;
    }
    return validationResult;
  }






  // createNewOttrData(e) {
  //   // //console.log((e);
  //   var temp = this.gridApi.getFilterModel();
  //   var temp2 = JSON.stringify(temp);
  //   if (temp2.trim() !== "{}") {
  //     this.disableCreateButton.emit(true);
  //   }
  //   else {
  //     this.disableCreateButton.emit(false);
  //   }
  //   var filterParams = this.gridApi.getFilterModel();
  //   this.gridApi.stopEditing();
  //   const ottrMonitoring = { ...e.rowNode.data };
  //   delete ottrMonitoring['difference'];

  //   // Nullify fields based on surveyFor value
  //   if (ottrMonitoring.surveyFor === 'External') {
  //     ottrMonitoring.fopmId = null;
  //     ottrMonitoring.fopmName = null;
  //   } else if (ottrMonitoring.surveyFor === 'Internal') {
  //     ottrMonitoring.exCustName = null;
  //     ottrMonitoring.exCustEmailId = null;
  //   }
  //   if (this.ottrService.getPMName() != null) {
  //     ottrMonitoring.gespm = this.ottrService.getPMName();
  //     this.ottrService.setPMName(null);
  //   }
  //   const validationStr = this.isValidOttrMonitoring(ottrMonitoring, ottrMonitoring.ottrId !== null);
  //   if (validationStr) {
  //     this.toastr.error(validationStr, 'Error');
  //     return;
  //   }
  //   if (!this.validateFOPMName(ottrMonitoring, e.rowIndex)) {
  //     return;
  //   }
  //   delete ottrMonitoring["delete"];
  //   if (ottrMonitoring.baseDate) {
  //     ottrMonitoring.baseDate = moment(ottrMonitoring.baseDate).format('YYYY-MM-DDTHH:mm:ss');
  //   }
  //   if (ottrMonitoring.planDate) {
  //     ottrMonitoring.planDate = moment(ottrMonitoring.planDate).format('YYYY-MM-DDTHH:mm:ss');
  //   }
  //   if (ottrMonitoring.actualDate) {
  //     ottrMonitoring.actualDate = moment(ottrMonitoring.actualDate).format('YYYY-MM-DDTHH:mm:ss');
  //   }
  //   ottrMonitoring.regionId = this.getRegionIdByRegionName(ottrMonitoring.regionId);
  //   ottrMonitoring.gesGroupId = this.getGESGroupIdByGESGroupName(ottrMonitoring.gesGroupId);
  //   if (ottrMonitoring.surveyRequest && ottrMonitoring.surveyRequest.indexOf("Required") > -1) {
  //     if (_isNumberValue(ottrMonitoring.spendPlan_Pole) == false) {
  //       //ottrMonitoring.spendPlan_Pole = Number(this.getKeyFromValue(poleData, ottrMonitoring.spendPlan_Pole));
  //       ottrMonitoring.spendPlan_Pole = ottrMonitoring.spendPlan_PoleId;
  //     }
  //     if (_isNumberValue(ottrMonitoring.spendPlan_Region) == false) {
  //       //ottrMonitoring.spendPlan_Region = Number(this.getKeyFromValue(spendPlanRegionData, ottrMonitoring.spendPlan_Region));
  //       ottrMonitoring.spendPlan_Region = ottrMonitoring.spendPlan_RegionId;
  //     }
  //     if (_isNumberValue(ottrMonitoring.spendPlan_Country) == false) {
  //       //ottrMonitoring.spendPlan_Country = Number(this.getKeyFromValue(spendPlanCountryData, ottrMonitoring.spendPlan_Country));
  //       ottrMonitoring.spendPlan_Country = ottrMonitoring.spendPlan_CountryId;
  //     }
  //   }
  //   else {
  //     ottrMonitoring.spendPlan_Pole = null;
  //     ottrMonitoring.spendPlan_Region = null;
  //     ottrMonitoring.spendPlan_Country = null;
  //     ottrMonitoring.surveyType = null;
  //     ottrMonitoring.gesResourceId = null;
  //     ottrMonitoring.gesResourceName = null;
  //   }
  //   if (ottrMonitoring.attributeToId !== null) {
  //     ottrMonitoring.attributeToId = this.getAttributeToIdByAttributeToName(ottrMonitoring.attributeToId);
  //   } else {
  //     delete ottrMonitoring["attributeToId"];
  //   }
  //   if (ottrMonitoring.internalExternalAttributeId !== null) {
  //     ottrMonitoring.internalExternalAttributeId = this.getInternalExternalAttributeIdByAttributeToName(ottrMonitoring.internalExternalAttributeId);
  //   } else {
  //     delete ottrMonitoring["internalExternalAttributeId"];
  //   }
  //   ottrMonitoring.productId = this.getProductIdByProductName(ottrMonitoring.productId);
  //   ottrMonitoring["isActive"] = true;
  //   const now = new Date();
  //   ottrMonitoring["modifiedBy"] = this.userID;
  //   ottrMonitoring["modifiedOn"] = now;

  //   if (ottrMonitoring.surveyRequest == "NA") {
  //     if (ottrMonitoring.fopmId == null || ottrMonitoring.fopmId == '' || ottrMonitoring.fopmId == undefined) {
  //       if (ottrMonitoring.ottrId === null) {
  //         ottrMonitoring["createdOn"] = now;
  //         ottrMonitoring["createdBy"] = this.userID;
  //         delete ottrMonitoring['ottrId'];
  //         this.spinner.show();
  //         this.ottrService.postOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {
  //           this.spinner.hide();
  //           if (data.StatusCode == 401 && data.data == "unauthorized access") {
  //             window.location.href = data.URL;
  //           } else if (data.Status == 401 && data.data == "unauthorized access") {
  //             window.location.href = data.URL;
  //           } else if (data.StatusCode == 400) {
  //             this.getOTTRList();
  //             this.toastr.error(JSON.parse(data.data).message, 'Error');
  //           }
  //           else {
  //             this.getOTTRList();
  //             this.addInProgress = false;
  //             this.toastr.success('Saved successfully', 'Done');
  //           }
  //         }, err => {
  //           console.error(err);
  //           this.toastr.error('Failed to save data due to server error!', 'Error');
  //           this.spinner.hide();
  //         });
  //         return;
  //       }
  //       this.spinner.show();
  //       this.ottrService.putOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {
  //         this.spinner.hide();
  //         if (data.StatusCode == 401 && data.data == "unauthorized access") {
  //           window.location.href = data.URL;
  //         } else if (data.Status == 401 && data.data == "unauthorized access") {
  //           window.location.href = data.URL;
  //         } else if (data.StatusCode == 400) {
  //           this.getOTTRList();
  //           this.toastr.error(JSON.parse(data.data).message, 'Error');
  //         }

  //         else {
  //           this.spinner.hide();
  //           this.getOTTRList();
  //           this.toastr.success('Saved successfully', 'Done');
  //           this.onSaveSuccess(e.rowIndex, e.rowNode);
  //         }
  //       }, err => {
  //         this.spinner.hide();
  //         console.error(err);
  //         this.toastr.error('Failed to save data due to server error!', 'Error');
  //       });
  //     }
  //     else {
  //       if (ottrMonitoring.fopmId.indexOf(";") > -1) {
  //         var fopmIdList = ottrMonitoring.fopmId.split(";");
  //         for (var item of fopmIdList) {
  //           if (item.length != 7) {
  //             this.toastr.error("FOPM EID should be 7 characters only", "Error");
  //             return;
  //           }
  //           var filteredId = fopmIdList.filter(x => x.toLowerCase() == item.toLowerCase());
  //           if (filteredId.length > 1) {
  //             this.toastr.error("Duplicate  EID are not allowed", "Error");
  //             return;
  //           }
  //         }
  //         if (fopmIdList.length > 5) {
  //           this.toastr.error("Maximum 5 FOPM EID are allowed", "Error");
  //           return;
  //         }

  //       }
  //       if (ottrMonitoring.fopmId.indexOf(";") == -1 && ottrMonitoring.fopmId.length != 7) {
  //         this.toastr.error("FOPM EID must be 7 characters", "Error");
  //         return;
  //       }
  //       this.spinner.show();
  //       this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(ottrMonitoring.fopmId).subscribe(userData => {
  //         if (userData.data != ottrMonitoring.fopmName) {
  //           this.spinner.hide();
  //           this.toastr.error("The FOPM Names do not match with FOPM ID please search again", "Error");
  //           return;
  //         }
  //         if (ottrMonitoring.ottrId === null) {
  //           ottrMonitoring["createdOn"] = now;
  //           ottrMonitoring["createdBy"] = this.userID;
  //           delete ottrMonitoring['ottrId'];
  //           this.spinner.show();
  //           this.ottrService.postOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {
  //             this.spinner.hide();
  //             if (data.StatusCode == 401 && data.data == "unauthorized access") {
  //               window.location.href = data.URL;
  //             } else if (data.Status == 401 && data.data == "unauthorized access") {
  //               window.location.href = data.URL;
  //             } else if (data.StatusCode == 400) {
  //               this.getOTTRList();
  //               this.toastr.error(JSON.parse(data.data).message, 'Error');
  //             }
  //             else {
  //               this.getOTTRList();
  //               this.addInProgress = false;
  //               this.toastr.success('Saved successfully', 'Done');
  //             }
  //           }, err => {
  //             console.error(err);
  //             this.toastr.error('Failed to save data due to server error!', 'Error');
  //             this.spinner.hide();
  //           });
  //           return;
  //         }
  //         this.ottrService.putOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {
  //           this.spinner.hide();
  //           if (data.StatusCode == 401 && data.data == "unauthorized access") {
  //             window.location.href = data.URL;
  //           } else if (data.Status == 401 && data.data == "unauthorized access") {
  //             window.location.href = data.URL;
  //           } else if (data.StatusCode == 400) {
  //             this.getOTTRList();
  //             this.toastr.error(JSON.parse(data.data).message, 'Error');
  //           }

  //           else {
  //             this.spinner.hide();
  //             this.getOTTRList();
  //             this.toastr.success('Saved successfully', 'Done');
  //             this.onSaveSuccess(e.rowIndex, e.rowNode);
  //           }
  //         }, err => {
  //           this.spinner.hide();
  //           console.error(err);
  //           this.toastr.error('Failed to save data due to server error!', 'Error');
  //         });

  //       })






  //     }
  //   }
  //   else {
  //     if (ottrMonitoring.fopmId.indexOf(";") > -1) {
  //       var fopmIdList = ottrMonitoring.fopmId.split(";");
  //       for (var item of fopmIdList) {
  //         if (item.length != 7) {
  //           this.toastr.error("FOPM EID should be 7 characters only", "Error");
  //           return;
  //         }
  //         var filteredId = fopmIdList.filter(x => x.toLowerCase() == item.toLowerCase());
  //         if (filteredId.length > 1) {
  //           this.toastr.error("Duplicate FOPM EID are not allowed", "Error");
  //           return;
  //         }
  //       }
  //       if (fopmIdList.length > 5) {
  //         this.toastr.error("Maximum 5 FOPM EID are allowed", "Error");
  //         return;
  //       }

  //     }
  //     if (ottrMonitoring.fopmId.indexOf(";") == -1 && ottrMonitoring.fopmId.length != 7) {
  //       this.toastr.error("FOPM EID must be 7 characters", "Error");
  //       return;
  //     }
  //     if (ottrMonitoring.surveyType == "Back Office") {
  //       ottrMonitoring.gesResourceId = null;
  //       ottrMonitoring.gesResourceName = null;
  //     }
  //     else {
  //       if (ottrMonitoring.gesResourceId !=null && ottrMonitoring.gesResourceId.length != 7) {
  //         this.toastr.error("GES Resource ID should be 7 characters", "Error");
  //         return;
  //       }
  //     }
  //     this.spinner.show();
  //     this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(ottrMonitoring.fopmId).subscribe(userData => {
  //       if (userData.data != ottrMonitoring.fopmName) {
  //         this.spinner.hide();
  //         this.toastr.error("The FOPM Names do not match with FOPM ID please search again", "Error");
  //         return;
  //       }
  //       if (ottrMonitoring.ottrId === null) {
  //         ottrMonitoring["createdOn"] = now;
  //         ottrMonitoring["createdBy"] = this.userID;
  //         delete ottrMonitoring['ottrId'];
  //         this.spinner.show();
  //         this.ottrService.postOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {
  //           this.spinner.hide();
  //           if (data.StatusCode == 401 && data.data == "unauthorized access") {
  //             window.location.href = data.URL;
  //           } else if (data.Status == 401 && data.data == "unauthorized access") {
  //             window.location.href = data.URL;
  //           } else if (data.StatusCode == 400) {
  //             this.getOTTRList();
  //             this.toastr.error(JSON.parse(data.data).message, 'Error');
  //           }
  //           else {
  //             this.getOTTRList();
  //             this.addInProgress = false;
  //             this.toastr.success('Saved successfully', 'Done');
  //           }
  //         }, err => {
  //           console.error(err);
  //           this.toastr.error('Failed to save data due to server error!', 'Error');
  //           this.spinner.hide();
  //         });
  //         return;
  //       }
  //       this.ottrService.putOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {
  //         this.spinner.hide();
  //         if (data.StatusCode == 401 && data.data == "unauthorized access") {
  //           window.location.href = data.URL;
  //         } else if (data.Status == 401 && data.data == "unauthorized access") {
  //           window.location.href = data.URL;
  //         } else if (data.StatusCode == 400) {
  //           this.getOTTRList();
  //           this.toastr.error(JSON.parse(data.data).message, 'Error');
  //         }

  //         else {
  //           this.spinner.hide();
  //           this.getOTTRList();
  //           this.toastr.success('Saved successfully', 'Done');
  //           this.onSaveSuccess(e.rowIndex, e.rowNode);
  //         }
  //       }, err => {
  //         this.spinner.hide();
  //         console.error(err);
  //         this.toastr.error('Failed to save data due to server error!', 'Error');
  //       });

  //     })



  //   }







  // }

  
 
createNewOttrData(e) {

  var temp = this.gridApi.getFilterModel();

  var temp2 = JSON.stringify(temp);

  if (temp2.trim() !== "{}") {

    this.disableCreateButton.emit(true);

  } else {

    this.disableCreateButton.emit(false);

  }

  var filterParams = this.gridApi.getFilterModel();

  this.gridApi.stopEditing();

  const ottrMonitoring = { ...e.rowNode.data };

  delete ottrMonitoring['difference'];
 
  // Nullify fields based on surveyFor value

  if (ottrMonitoring.surveyFor === 'External') {

    ottrMonitoring.fopmId = null;

    ottrMonitoring.fopmName = null;

  } else if (ottrMonitoring.surveyFor === 'Internal') {

    ottrMonitoring.exCustName = null;

    ottrMonitoring.exCustEmailId = null;

  }

  if (this.ottrService.getPMName() != null) {

    ottrMonitoring.gespm = this.ottrService.getPMName();

    this.ottrService.setPMName(null);

  }

  const validationStr = this.isValidOttrMonitoring(ottrMonitoring, ottrMonitoring.ottrId !== null);

  if (validationStr) {

    this.toastr.error(validationStr, 'Error');

    return;

  }

  if (!this.validateFOPMName(ottrMonitoring, e.rowIndex)) {

    return;

  }

  delete ottrMonitoring["delete"];

  if (ottrMonitoring.baseDate) {

    ottrMonitoring.baseDate = moment(ottrMonitoring.baseDate).format('YYYY-MM-DDTHH:mm:ss');

  }

  if (ottrMonitoring.planDate) {

    ottrMonitoring.planDate = moment(ottrMonitoring.planDate).format('YYYY-MM-DDTHH:mm:ss');

  }

  if (ottrMonitoring.actualDate) {

    ottrMonitoring.actualDate = moment(ottrMonitoring.actualDate).format('YYYY-MM-DDTHH:mm:ss');

  }

  ottrMonitoring.regionId = this.getRegionIdByRegionName(ottrMonitoring.regionId);

  ottrMonitoring.gesGroupId = this.getGESGroupIdByGESGroupName(ottrMonitoring.gesGroupId);

  if (ottrMonitoring.surveyRequest && ottrMonitoring.surveyRequest.indexOf("Required") > -1) {

    if (_isNumberValue(ottrMonitoring.spendPlan_Pole) == false) {

      ottrMonitoring.spendPlan_Pole = ottrMonitoring.spendPlan_PoleId;

    }

    if (_isNumberValue(ottrMonitoring.spendPlan_Region) == false) {

      ottrMonitoring.spendPlan_Region = ottrMonitoring.spendPlan_RegionId;

    }

    if (_isNumberValue(ottrMonitoring.spendPlan_Country) == false) {

      ottrMonitoring.spendPlan_Country = ottrMonitoring.spendPlan_CountryId;

    }

  } else {

    ottrMonitoring.spendPlan_Pole = null;

    ottrMonitoring.spendPlan_Region = null;

    ottrMonitoring.spendPlan_Country = null;

    ottrMonitoring.surveyType = null;

    ottrMonitoring.gesResourceId = null;

    ottrMonitoring.gesResourceName = null;

  }

  if (ottrMonitoring.attributeToId !== null) {

    ottrMonitoring.attributeToId = this.getAttributeToIdByAttributeToName(ottrMonitoring.attributeToId);

  } else {

    delete ottrMonitoring["attributeToId"];

  }

  if (ottrMonitoring.internalExternalAttributeId !== null) {

    ottrMonitoring.internalExternalAttributeId = this.getInternalExternalAttributeIdByAttributeToName(ottrMonitoring.internalExternalAttributeId);

  } else {

    delete ottrMonitoring["internalExternalAttributeId"];

  }

  ottrMonitoring.productId = this.getProductIdByProductName(ottrMonitoring.productId);

  ottrMonitoring["isActive"] = true;

  const now = new Date();

  ottrMonitoring["modifiedBy"] = this.userID;

  ottrMonitoring["modifiedOn"] = now;
 
  // Handle Survey Request = "NA"

  if (ottrMonitoring.surveyRequest == "NA") {

    // Check if fopmId is null or empty (External survey case)

    if (ottrMonitoring.fopmId == null || ottrMonitoring.fopmId == '' || ottrMonitoring.fopmId == undefined) {

      this.saveOttrData(ottrMonitoring, now, e);

      return;

    }

    // Validate FOPM ID format for Internal survey

    if (ottrMonitoring.fopmId.indexOf(";") > -1) {

      var fopmIdList = ottrMonitoring.fopmId.split(";");

      for (var item of fopmIdList) {

        if (item.length != 7) {

          this.toastr.error("FOPM EID should be 7 characters only", "Error");

          return;

        }

        var filteredId = fopmIdList.filter(x => x.toLowerCase() == item.toLowerCase());

        if (filteredId.length > 1) {

          this.toastr.error("Duplicate EID are not allowed", "Error");

          return;

        }

      }

      if (fopmIdList.length > 5) {

        this.toastr.error("Maximum 5 FOPM EID are allowed", "Error");

        return;

      }

    }

    if (ottrMonitoring.fopmId.indexOf(";") == -1 && ottrMonitoring.fopmId.length != 7) {

      this.toastr.error("FOPM EID must be 7 characters", "Error");

      return;

    }

    // Validate FOPM Name matches EID

    this.spinner.show();

    this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(ottrMonitoring.fopmId).subscribe(userData => {

      if (userData.data != ottrMonitoring.fopmName) {

        this.spinner.hide();

        this.toastr.error("The FOPM Names do not match with FOPM ID please search again", "Error");

        return;

      }

      this.saveOttrData(ottrMonitoring, now, e);

    });

    return;

  }

  // Handle Survey Request = "Required"

  // Only validate FOPM if it's an Internal survey

  if (ottrMonitoring.surveyFor === "Internal" && 

      ottrMonitoring.fopmId != null && 

      ottrMonitoring.fopmId != '' && 

      ottrMonitoring.fopmId != undefined) {

    if (ottrMonitoring.fopmId.indexOf(";") > -1) {

      var fopmIdList = ottrMonitoring.fopmId.split(";");

      for (var item of fopmIdList) {

        if (item.length != 7) {

          this.toastr.error("FOPM EID should be 7 characters only", "Error");

          return;

        }

        var filteredId = fopmIdList.filter(x => x.toLowerCase() == item.toLowerCase());

        if (filteredId.length > 1) {

          this.toastr.error("Duplicate FOPM EID are not allowed", "Error");

          return;

        }

      }

      if (fopmIdList.length > 5) {

        this.toastr.error("Maximum 5 FOPM EID are allowed", "Error");

        return;

      }

    }

    if (ottrMonitoring.fopmId.indexOf(";") == -1 && ottrMonitoring.fopmId.length != 7) {

      this.toastr.error("FOPM EID must be 7 characters", "Error");

      return;

    }

  }

  // Handle GES Resource ID validation for Front Office

  if (ottrMonitoring.surveyType == "Back Office") {

    ottrMonitoring.gesResourceId = null;

    ottrMonitoring.gesResourceName = null;

  } else if (ottrMonitoring.surveyType == "Front Office") {

    if (ottrMonitoring.gesResourceId != null && 

        ottrMonitoring.gesResourceId != '' && 

        ottrMonitoring.gesResourceId != undefined && 

        ottrMonitoring.gesResourceId.length != 7) {

      this.toastr.error("GES Resource ID should be 7 characters", "Error");

      return;

    }

  }

  // For Internal surveys with FOPM, validate against PeopleSoft API

  if (ottrMonitoring.surveyFor === "Internal" && 

      ottrMonitoring.fopmId != null && 

      ottrMonitoring.fopmId != '' && 

      ottrMonitoring.fopmId != undefined) {

    this.spinner.show();

    this.ottrService.getEmployeeNameFromEIDPeopleSoftAPI(ottrMonitoring.fopmId).subscribe(userData => {

      if (userData.data != ottrMonitoring.fopmName) {

        this.spinner.hide();

        this.toastr.error("The FOPM Names do not match with FOPM ID please search again", "Error");

        return;

      }

      this.saveOttrData(ottrMonitoring, now, e);

    });

  } else {

    // For External surveys or surveys without FOPM, save directly

    this.saveOttrData(ottrMonitoring, now, e);

  }

}
 
// Helper method to save OTTR data

saveOttrData(ottrMonitoring: any, now: Date, e: any) {

  if (ottrMonitoring.ottrId === null) {

    ottrMonitoring["createdOn"] = now;

    ottrMonitoring["createdBy"] = this.userID;

    delete ottrMonitoring['ottrId'];

    this.spinner.show();

    this.ottrService.postOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {

      this.spinner.hide();

      if (data.StatusCode == 401 && data.data == "unauthorized access") {

        window.location.href = data.URL;

      } else if (data.Status == 401 && data.data == "unauthorized access") {

        window.location.href = data.URL;

      } else if (data.StatusCode == 400) {

        this.getOTTRList();

        this.toastr.error(JSON.parse(data.data).message, 'Error');

      } else {

        this.getOTTRList();

        this.addInProgress = false;

        this.toastr.success('Saved successfully', 'Done');

      }

    }, err => {

      console.error(err);

      this.toastr.error('Failed to save data due to server error!', 'Error');

      this.spinner.hide();

    });

  } else {

    this.spinner.show();

    this.ottrService.putOttrMonitoringData(ottrMonitoring).subscribe((data: any) => {

      this.spinner.hide();

      if (data.StatusCode == 401 && data.data == "unauthorized access") {

        window.location.href = data.URL;

      } else if (data.Status == 401 && data.data == "unauthorized access") {

        window.location.href = data.URL;

      } else if (data.StatusCode == 400) {

        this.getOTTRList();

        this.toastr.error(JSON.parse(data.data).message, 'Error');

      } else {

        this.spinner.hide();

        this.getOTTRList();

        this.toastr.success('Saved successfully', 'Done');

        this.onSaveSuccess(e.rowIndex, e.rowNode);

      }

    }, err => {

      this.spinner.hide();

      console.error(err);

      this.toastr.error('Failed to save data due to server error!', 'Error');

    });

  }

}
 

  cancelOttrData(e) {
    this.copyFlag = !this.EditRole;
    this.gridApi.stopEditing();
    this.disableCreateButton.emit(!this.EditRole);
    var temp = this.gridApi.getFilterModel();
    var temp2 = JSON.stringify(temp);
    if (temp2.trim() !== "{}") {
      this.disableCreateButton.emit(true);
    }
    else {
      this.disableCreateButton.emit(false);
    }
    this.modifiedRowIndex = [];
    this.addInProgress = false;
    this.spinner.show();
    if (e.rowNode.data.ottrId != null && e.rowNode.data.ottrId !== '') {
      this.ottrService.getOTTRMonitoringDataById(e.rowNode.data.ottrId).subscribe(data => {
        for (var rowItem of this.rowData) {
          if (rowItem.ottrId == e.rowNode.data.ottrId) {
            rowItem.spendPlan_Pole = data.spendPlan_Pole;
            rowItem.spendPlan_Region = data.spendPlan_Region;
            rowItem.spendPlan_Country = data.spendPlan_Country;
            rowItem.productId = data.productId;
            rowItem.projectId = data.projectId;
            rowItem.projectName = data.projectName;
            rowItem.fopmId = data.fopmId;
            rowItem.attributeToId = data.attributeToId;
            rowItem.fopmName = data.fopmName;
            rowItem.gesGroupId = data.gesGroupId;
            rowItem.gespm = data.gespm;
            rowItem.gespmEID = data.gespmEID;
            rowItem.baseDate = data.baseDate;
            rowItem.planDate = data.planDate;
            rowItem.actualDate = data.actualDate;
            rowItem.regionId = data.regionId;
            rowItem.comments = data.comments;
            rowItem.deliverables = data.deliverables;
            rowItem.surveyRequest = data.surveyRequest;
            rowItem.surveyType = data.surveyType;
            rowItem.gesResourceId = data.gesResourceId;
            rowItem.gesResourceName = data.gesResourceName;
            rowItem.surveyFor=data.surveyFor
            rowItem.exCustEmailId=data.exCustEmailId
            rowItem.exCustName=data.exCustName
            rowItem.selectedRow = false;
            rowItem.internalExternalAttributeId = data.internalExternalAttributeId;
            rowItem.trackingToDesignInput = data.trackingToDesignInput;
            rowItem.spendPlan_PoleId = data.spendPlan_Pole;
            rowItem.spendPlan_RegionId = data.spendPlan_Region;
            rowItem.spendPlan_CountryId = data.spendPlan_Country;
            
            break;
          }
        }
        this.ottrMonitoringData = null;
        if (e.rowNode.data.ottrId == null || e.rowNode.data.ottrId === '') {
          this.rowData.splice(e.rowIndex, 1);
        }
        this.rowData = [...this.rowData];
        this.planDateCommentFlag = this.baseDateCommentFlag = false;
        this.spinner.hide();
      });
    }
    else {
      if (e.rowNode.data.ottrId == null || e.rowNode.data.ottrId === '') {
        this.rowData.splice(e.rowIndex, 1);
      }
      this.rowData = [...this.rowData];
      this.planDateCommentFlag = this.baseDateCommentFlag = false;
      this.spinner.hide();
    }


  }

  //Ottr monitoring validator
//   isValidOttrMonitoring(data, isExistingRecord: boolean) {
//     try {
//       if(Number(data['deliveryHours']) < Number(data['reworkHours']))
//       {
//         return "Rework Hours should be always less than Delivery Hours.";        
//       }
//       if ((data.attributeToId == attributeToData[3] || data.attributeToId == 3) && !data['comments']) {
//         return "Comments are mandatory if Attribute to is Delayed-On Hold";
//       }
//       var params = [];
//       params['data'] = data;
//       var aa = this.getDefaultAttribute(params);
//       if (data.actualDate == null && (data.attributeToId == attributeToData[4] || data.attributeToId == 4 || aa == 4)) {
//         data.attributeToId = 0;
//       }

//       if (data.attributeToId == attributeToData[4] || data.attributeToId == 4 || aa == 4) {
//         data.attributeToId = 4;
//       } else {
//         if (aa != 0 && data.attributeToId != null && !(data.attributeToId == attributeToData[4] || data.attributeToId == 4) && data['internalExternalAttributeId'] == undefined) {
//           return "Internal / External Attribute is mandatory if Attribute to is not On Time";
//         } else if (!data.attributeToId)
//           if (aa != 0 && aa != 4 && data['internalExternalAttributeId'] == undefined) {
//             return "Internal / External Attribute is mandatory if Attribute to is not On Time";
//           }
//       }
//       if (isExistingRecord) {
//         if (this.baseDateCommentFlag == true && !data['comments']) {
//           return "Comments are mandatory if Base date is changed";
//         }
//         if (this.planDateCommentFlag == true && !data['comments']) {
//           return "Comments are mandatory if Plan date is changed";
//         }
//       }
//       if (this.calculateDifference({ data }) < 0 && (!(data['comments']) || !(data['attributeToId']))) {
//         return "Comments and Attribute To are mandatory for delayed deliverable!";
//       }
//       for (const key in data) {
//         if (key === 'ottrId' || key === 'attributeToId' || key === 'actualDate' || key==='actualDateModifiedBy' || key==='actualDateModifiedOn'|| key === 'customerSurveyTemplate' || key === 'customerSurvey' ||
//           key === 'fopmName' || key === 'fopmId' || key === 'trackingToDesignInput' || key === 'internalExternalAttributeId' || key === 'comments') {
//           continue;
//         }
//         if (key === "month") {
//           data["month"] = this.calculateMonth(data);
//         }
//         // if (key === "surveyRequest") {
//         //   if (data["surveyRequest"].indexOf("Required") > -1 && (data["surveyType"] == "" || data["surveyType"] == null || data["surveyType"] == undefined)) {
//         //     return "Survey type is mandatory of Survey Request is required";
//         //   }
//         //   if (data["surveyRequest"].indexOf("Required") > -1 && data["surveyType"].indexOf("Front Office") > -1 && (data["gesResourceId"] == null || data["gesResourceId"] == undefined || data["gesResourceId"] == "")) {
//         //     return "GES Resource EID is mandatory if Survey Request is Required and Survey Type is Front Office";
//         //   }
//         //   if (data["surveyRequest"].indexOf("Required") > -1 &&
//         //     (data["fopmId"] != null && data["fopmId"] != undefined && data["fopmId"] != "")
//         //     && (data["fopmName"] == null || data["fopmName"] == undefined || data["fopmName"] == "")
//         //   ) {
//         //     return "FOPM Name is mandatory if Survey Request is Required.Please enter valid FOPM EID to get FOPM Names";
//         //   }
//         //   if (data["surveyRequest"] == "NA" && (data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined)
//         //     && (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined)
//         //     && (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined)
//         //     && (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == "")) {
//         //     continue;
//         //   }
//         //   if (data["surveyRequest"].indexOf("Required") > -1 && ((data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined)
//         //     || (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined)
//         //     || (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined)
//         //     || (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == ""))) {
//         //     if (data["surveyRequest"].indexOf("Required") > -1 && (data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined)) {
//         //       return "Pole is mandatory if Survey Request is Required";
//         //     }
//         //     if (data["surveyRequest"].indexOf("Required") > -1 && (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined)) {
//         //       return "Region is mandatory if Survey Request is Required";
//         //     }
//         //     if (data["surveyRequest"].indexOf("Required") > -1 && (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined)) {
//         //       return "Country is mandatory if Survey Request is Required";
//         //     }
//         //     if (data["surveyRequest"].indexOf("Required") > -1 && (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == "")&&  (data["surveyFor "]=== 'Internal')) {
//         //       return "FOPM EID is mandatory if Survey Request is Required";
//         //     }
//         //      if (data["surveyRequest"].indexOf("Required") > -1 && (data["exCustName"] == null || data["exCustName"] == undefined || data["exCustName"] == "")&&  (data["surveyFor "]=== 'External')) {
//         //       return "External Customer Name is mandatory if Survey Request is Required";
//         //     }
//         //      if (data["surveyRequest"].indexOf("Required") > -1 && (data["exCustEmailId"] == null || data["exCustEmailId"] == undefined || data["exCustEmailId"] == "")&&  (data["surveyFor "]=== 'External')) {
//         //       return "External Customer EmailId is mandatory if Survey Request is Required";
//         //     }
//         //     if (data["surveyRequest"].indexOf("Required") > -1 && (data["gesResourceId"].length != 7)) {
//         //       return "GES Resource EID should be 7 characters";
//         //     }
//         //   }
//         // }
//         // Replace the validation section in isValidOttrMonitoring method (around lines 3990-4025)

// if (key === "surveyRequest") {
//   if (data["surveyRequest"].indexOf("Required") > -1 && (data["surveyType"] == "" || data["surveyType"] == null || data["surveyType"] == undefined)) {
//     return "Survey type is mandatory if Survey Request is required";
//   }
//   if (data["surveyRequest"].indexOf("Required") > -1 && data["surveyType"].indexOf("Front Office") > -1 && (data["gesResourceId"] == null || data["gesResourceId"] == undefined || data["gesResourceId"] == "")) {
//     return "GES Resource EID is mandatory if Survey Request is Required and Survey Type is Front Office";
//   }
  
//   // Check for Internal survey
//   if (data["surveyRequest"].indexOf("Required") > -1 && 
//       data["surveyFor"] === "Internal" &&
//       (data["fopmId"] != null && data["fopmId"] != undefined && data["fopmId"] != "") &&
//       (data["fopmName"] == null || data["fopmName"] == undefined || data["fopmName"] == "")) {
//     return "FOPM Name is mandatory if Survey Request is Required. Please enter valid FOPM EID to get FOPM Names";
//   }
  
//   // Check for External survey
//   if (data["surveyRequest"].indexOf("Required") > -1 && 
//       data["surveyFor"] === "External" &&
//       (data["exCustName"] == null || data["exCustName"] == undefined || data["exCustName"] == "")) {
//     return "External Customer Name is mandatory if Survey Request is Required and Survey For is External";
//   }
  
//   if (data["surveyRequest"].indexOf("Required") > -1 && 
//       data["surveyFor"] === "External" &&
//       (data["exCustEmailId"] == null || data["exCustEmailId"] == undefined || data["exCustEmailId"] == "")) {
//     return "External Customer Email ID is mandatory if Survey Request is Required and Survey For is External";
//   }
  
//   if (data["surveyRequest"] == "NA" && 
//       (data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined) &&
//       (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined) &&
//       (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined) &&
//       (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == "")) {
//     continue;
//   }
  
//   if (data["surveyRequest"].indexOf("Required") > -1) {
//     if (data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined) {
//       return "Pole is mandatory if Survey Request is Required";
//     }
//     if (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined) {
//       return "Region is mandatory if Survey Request is Required";
//     }
//     if (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined) {
//       return "Country is mandatory if Survey Request is Required";
//     }
    
//     // FOPM EID check - only for Internal
//     if (data["surveyFor"] === "Internal" && 
//         (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == "")) {
//       return "FOPM EID is mandatory if Survey Request is Required and Survey For is Internal";
//     }
    
//     // GES Resource ID validation - only check length if not null
//     if (data["surveyType"] && data["surveyType"].indexOf("Front Office") > -1 &&
//         data["gesResourceId"] != null && 
//         data["gesResourceId"] != undefined && 
//         data["gesResourceId"] != "" && 
//         data["gesResourceId"].length != 7) {
//       return "GES Resource EID should be 7 characters";
//     }
//   }
// }
//         else {
//           if ((data[key] === null || data[key] === '' || data[key] === undefined)
//             && (key != "spendPlan_Pole"
//               && key != "spendPlan_Region"
//               && key != "spendPlan_Country"
//               && key != "surveyType"
//               && key != "gesResourceId"
//               && key != "gesResourceName"
//               && key != "fopmEmailID"
//               && key != "spendPlan_PoleId"
//               && key != "spendPlan_RegionId"
//               && key != "spendPlan_CountryId"
//               && key != "delete"
//               && key != "createdByEmail"
//               && key != "createdByManagerEmailId"
//               && key != "gespmManagerEmail"
//               && key != "gespmEmail"
//               && key!="surveyFor"
//               && key!="exCustEmailId"
//               && key!="exCustName"
//             )
//           ) {
//             return "Please fill the mandatory fields!";
//           }
//         }
//       }
//       if (!(gesGroupProductMappingData[this.getGESGroupIdByGESGroupName(data.gesGroupId)].includes(this.getProductIdByProductName(data.productId)))) {
//         return "Please enter the appropriate Product for the selected GES Group!";
//       }
//       if (data["gespmEID"].length != 7) {
//         return "GES PM/EM EID should be 7 characters";
//       }
//       return null;
//     } catch (err) {
//       console.error(err);
//       return "Validation Failed!";
//     }
//   }

isValidOttrMonitoring(data, isExistingRecord: boolean) {

  try {

    // Delivery vs Rework hours validation

    if (Number(data['deliveryHours']) < Number(data['reworkHours'])) {

      return "Rework Hours should be always less than Delivery Hours.";

    }

    // Comments validation for Delayed-On Hold

    if ((data.attributeToId == attributeToData[3] || data.attributeToId == 3) && !data['comments']) {

      return "Comments are mandatory if Attribute to is Delayed-On Hold";

    }

    // Attribute validation

    var params = [];

    params['data'] = data;

    var aa = this.getDefaultAttribute(params);

    if (data.actualDate == null && (data.attributeToId == attributeToData[4] || data.attributeToId == 4 || aa == 4)) {

      data.attributeToId = 0;

    }
 
    if (data.attributeToId == attributeToData[4] || data.attributeToId == 4 || aa == 4) {

      data.attributeToId = 4;

    } else {

      if (aa != 0 && data.attributeToId != null && !(data.attributeToId == attributeToData[4] || data.attributeToId == 4) && data['internalExternalAttributeId'] == undefined) {

        return "Internal / External Attribute is mandatory if Attribute to is not On Time";

      } else if (!data.attributeToId) {

        if (aa != 0 && aa != 4 && data['internalExternalAttributeId'] == undefined) {

          return "Internal / External Attribute is mandatory if Attribute to is not On Time";

        }

      }

    }

    // Existing record validations

    if (isExistingRecord) {

      if (this.baseDateCommentFlag == true && !data['comments']) {

        return "Comments are mandatory if Base date is changed";

      }

      if (this.planDateCommentFlag == true && !data['comments']) {

        return "Comments are mandatory if Plan date is changed";

      }

    }

    // Delayed deliverable validation

    if (this.calculateDifference({ data }) < 0 && (!(data['comments']) || !(data['attributeToId']))) {

      return "Comments and Attribute To are mandatory for delayed deliverable!";

    }

    // Main field validation loop

    for (const key in data) {

      // Skip non-mandatory fields

      if (key === 'ottrId' || key === 'attributeToId' || key === 'actualDate' || 

          key === 'actualDateModifiedBy' || key === 'actualDateModifiedOn' || 

          key === 'customerSurveyTemplate' || key === 'customerSurvey' ||

          key === 'fopmName' || key === 'fopmId' || key === 'trackingToDesignInput' || 

          key === 'internalExternalAttributeId' || key === 'comments') {

        continue;

      }

      // Calculate month

      if (key === "month") {

        data["month"] = this.calculateMonth(data);

      }

      // Survey Request specific validations

      if (key === "surveyRequest") {

        // Survey type required if Survey Request is Required

        if (data["surveyRequest"].indexOf("Required") > -1 && 

            (data["surveyType"] == "" || data["surveyType"] == null || data["surveyType"] == undefined)) {

          return "Survey type is mandatory if Survey Request is required";

        }

        // GES Resource EID required for Front Office

        if (data["surveyRequest"].indexOf("Required") > -1 && 

            data["surveyType"] && data["surveyType"].indexOf("Front Office") > -1 && 

            (data["gesResourceId"] == null || data["gesResourceId"] == undefined || data["gesResourceId"] == "")) {

          return "GES Resource EID is mandatory if Survey Request is Required and Survey Type is Front Office";

        }

        // FOPM Name validation for Internal surveys (only if fopmId exists)

        if (data["surveyRequest"].indexOf("Required") > -1 &&

            data["surveyFor"] === "Internal" &&

            (data["fopmId"] != null && data["fopmId"] != undefined && data["fopmId"] != "") &&

            (data["fopmName"] == null || data["fopmName"] == undefined || data["fopmName"] == "")) {

          return "FOPM Name is mandatory if Survey Request is Required. Please enter valid FOPM EID to get FOPM Names";

        }

        // Skip if Survey Request is NA and optional fields are empty

        if (data["surveyRequest"] == "NA" && 

            (data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined) &&

            (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined) &&

            (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined) &&

            (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == "")) {

          continue;

        }

        // Required validations when Survey Request is "Required"

        if (data["surveyRequest"].indexOf("Required") > -1) {

          // Always required fields

          if (data["spendPlan_Pole"] == null || data["spendPlan_Pole"] == undefined) {

            return "Pole is mandatory if Survey Request is Required";

          }

          if (data["spendPlan_Region"] == null || data["spendPlan_Region"] == undefined) {

            return "Region is mandatory if Survey Request is Required";

          }

          if (data["spendPlan_Country"] == null || data["spendPlan_Country"] == undefined) {

            return "Country is mandatory if Survey Request is Required";

          }

          // Internal survey specific validations

          if (data["surveyFor"] === "Internal") {

            if (data["fopmId"] == null || data["fopmId"] == undefined || data["fopmId"] == "") {

              return "FOPM EID is mandatory if Survey Request is Required and Survey For is Internal";

            }

          }

          // External survey specific validations

          if (data["surveyFor"] === "External") {

            if (data["exCustName"] == null || data["exCustName"] == undefined || data["exCustName"] == "") {

              return "External Customer Name is mandatory if Survey Request is Required and Survey For is External";

            }

            if (data["exCustEmailId"] == null || data["exCustEmailId"] == undefined || data["exCustEmailId"] == "") {

              return "External Customer Email ID is mandatory if Survey Request is Required and Survey For is External";

            }

          }

          // GES Resource ID length validation (only if it has a value)

          if (data["gesResourceId"] != null && 

              data["gesResourceId"] != undefined && 

              data["gesResourceId"] != "" && 

              data["gesResourceId"].length != 7) {

            return "GES Resource EID should be 7 characters";

          }

        }

      } else {

        // General mandatory field validation

        if ((data[key] === null || data[key] === '' || data[key] === undefined) &&

            key != "spendPlan_Pole" &&

            key != "spendPlan_Region" &&

            key != "spendPlan_Country" &&

            key != "surveyType" &&

            key != "gesResourceId" &&

            key != "gesResourceName" &&

            key != "fopmEmailID" &&

            key != "spendPlan_PoleId" &&

            key != "spendPlan_RegionId" &&

            key != "spendPlan_CountryId" &&

            key != "delete" &&

            key != "createdByEmail" &&

            key != "createdByManagerEmailId" &&

            key != "gespmManagerEmail" &&

            key != "gespmEmail" &&

            key != "surveyFor" &&

            key != "exCustEmailId" &&

            key != "exCustName") {

          return "Please fill the mandatory fields!";

        }

      }

    }

    // Product validation

    if (!(gesGroupProductMappingData[this.getGESGroupIdByGESGroupName(data.gesGroupId)].includes(this.getProductIdByProductName(data.productId)))) {

      return "Please enter the appropriate Product for the selected GES Group!";

    }

    // GES PM/EM EID validation

    if (data["gespmEID"] && data["gespmEID"].length != 7) {

      return "GES PM/EM EID should be 7 characters";

    }

    return null;

  } catch (err) {

    console.error(err);

    return "Validation Failed!";

  }

}
}
