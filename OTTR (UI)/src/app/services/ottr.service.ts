import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Region } from '../models/Region';
import { OttrMonitoring } from '../models/OttrMonitoring';
import * as moment from 'moment';
import * as momentBusinessDays from '../static_code/index';
import * as _ from 'lodash';
import { GESGroup } from '../models/GESGroup';
import { AttributeTo } from '../models/AttributeTo';
import { InternalExternalAttributeTo } from '../models/InternalExternalAttributeTo';
import { Product } from '../models/Product';
import { GESGroupProductMapping } from '../models/GESGroupProductMapping';
import { environment } from 'src/environments/environment';
import { CookieService } from 'ngx-cookie-service';
import { BehaviorSubject } from 'rxjs';
import { Pole } from '../models/Pole';
import { SpendPlanRegion } from '../models/SpendPlanRegion';
import { Country } from '../models/Country';

@Injectable({
  providedIn: 'root'
})
export class OttrService {

  url: string;
  fileProcessorUrl: string;
  cookieValue: any;
  exportData = new BehaviorSubject(null);
  gesGroupChangeEvent = new BehaviorSubject(-1);
  poleChangeEvent = new BehaviorSubject(null);
  regionChangeEvent = new BehaviorSubject(null);
  surveyRequestChangeEvent = new BehaviorSubject(null);
  allSpendPlanRegionData: SpendPlanRegion[];
  allSpendPlanCountryData: Country[];
  gesGroupData: {};
  countryChangeEvent = new BehaviorSubject(null);
  uploadExcelEvent = new BehaviorSubject(null);
  unauthorizedAccess = new BehaviorSubject(null);
  allPoleData: Pole[];
  pmName:any;
  userCountry:any;
  allOptionSelected:boolean;
  selectedGESLOB:any;
  filterClearEvent = new BehaviorSubject(null);
  ottrDeleteId:any=[];
	roleList: any;
  setSelectedGESLOB(gesLOB)
  {
    this.selectedGESLOB = gesLOB;
  }

  
  getSelectedGESLOB()
  {
    return this.selectedGESLOB;
  }


  getAllOptionselected(){
    return this.allOptionSelected;
  }

  setAllOptionSelected(allOptionValue){
    this.allOptionSelected = allOptionValue;
  }
  setUserCountry(country){
    this.userCountry = country;
  }
  getUserCountry(){
    return this.userCountry;
  }
  getPMName(){
    return this.pmName;
  }
  setPMName(pm)
  {
    this.pmName = pm;
  }
  constructor(private http: HttpClient, private cookieService: CookieService) {
    this.url = environment.apiUrl;
    this.fileProcessorUrl = environment.fileProcessorUrl;
    momentBusinessDays.updateLocale('us', {
      workingWeekdays: [1, 2, 3, 4, 5]
    });
    this.cookieValue = this.cookieService.get('userEID');
  }

  getUserDetails(userId) {
    const requstProcessorData = {
      "app": "Security",
      "action": `/api/Employee/${userId}`,
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData);
  }


  getOTTRMonitoringData(gesGroup) {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/OttrMonitoring/GetAll/" + gesGroup+'/'+this.getUserCountry(),
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {  
      if(res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }    
      if(res.length > 0)
      {
        for (const row of res) {
          var a = momentBusinessDays(row.planDate);
          var b = momentBusinessDays(row.actualDate);
          row['difference'] = a.businessDiff(b, true);  
          row['spendPlan_PoleId'] = row['spendPlan_Pole'];
          row['spendPlan_CountryId'] = row['spendPlan_Country'];
          row['spendPlan_RegionId'] = row['spendPlan_Region'];     
        }
        return res;
      }      
    }));
  }

  getOTTRMonitoringDataById(ottrID) {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/OttrMonitoring/" + ottrID,
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {     
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      } 
      return res;
    }));
  }

  getRegionData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/Gesgroupdelivery",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {      
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      } 
      //console.log("region response for /api/Gesgroupdelivery", res);
      const regionObj = {};
      // var data = res.data;

      for (const row of res) {

        regionObj[row.regionId] = row.regionName;
      }
      return regionObj;
    }));
  }

  getPoleData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/Pole",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      this.allPoleData = res;
      const poleObj = {};
      // var data = res.data;

      for (const row of res) {

        poleObj[row.poleID] = row.poleName;
      }
      return poleObj;
    }));
  }

  getAllSpendPlanRegions() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/Region",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      this.allSpendPlanRegionData = res;
      const spendPlanRegionObj = {};
      for (const row of res) {
        spendPlanRegionObj[row.regionID] = row.regionName;
      }
      //console.log(this.allSpendPlanRegionData);
      return spendPlanRegionObj;
    }));
  }

  getSpendPlanRegionData(poleId) {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/Region/" + poleId,
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      const spendPlanRegionObj = {};
      // var data = res.data;     
      for (const row of res) {
        spendPlanRegionObj[row.regionID] = row.regionName;
      }
      return spendPlanRegionObj;
    }));
  }

  getSpendPlanCountryData(regionId) {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/RegionCountryMapping/" + regionId,
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      //console.log("region response", res);
      const CountryObj = {};
      // var data = res.data;     
      for (const row of res) {
        CountryObj[row.countryID] = row.countryName;
      }
      return CountryObj;
    }));
  }

  getAllSpendPlanCountryData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/Country",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }

      this.allSpendPlanCountryData = res;
      const CountryObj = {};
      // var data = res.data;     
      for (const row of res) {
        CountryObj[row.countryID] = row.countryName;
      }
      return CountryObj;
    }));
  }


  getGESGroupForSelection() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/GESGroup",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      return res;
    }));

  }


  getAllGESGroupData()
  {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/GESGroup/GetSubGroupWithGroup",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {      
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      return res;
    })); 
  }


  getGESGroupData(groupName) {        
    if(groupName != "All" && groupName!=undefined)
    {
      const requstProcessorData = {
        "app": "OTTR",
        "action": "/api/GESGroup/GetSubGroup/" + groupName,
        "requestType": "GET"
      }
      return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
        if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
        {
          this.triggerUnauthorizedAccessEvent(res);
          return;
        }
        const gesGroupObj = {};
        for (const row of res) {
          gesGroupObj[row.gesGroupId] = row.gesSubGroupName;
        }
        this.gesGroupData = gesGroupObj;
        //console.log(this.gesGroupData);
        return gesGroupObj;
      }));
    }
    else
    {
      const requstProcessorData = {
        "app": "OTTR",
        "action": "/api/GESGroup/GetSubGroupWithGroup",
        "requestType": "GET"
      }
      return this.http.post(this.url, requstProcessorData).pipe(map((res:any) => {  
        if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
        {
         this.triggerUnauthorizedAccessEvent(res);
         return;
        }
        //console.log(res);
        const gesGroupObj = {};
        for (const row of res) {
          for(var temp of row.gesSubGroup)
          {
            gesGroupObj[temp.subgroupId] = temp.subgroupName;
          }          
        }
        this.gesGroupData = gesGroupObj;
        return gesGroupObj;
      }));
    }
  }

  

  getAttributeToData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/AttributeTo",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      const attributeToObj = {};
      for (const row of res) {
        attributeToObj[row.attributeToId] = row.attributeToName;
      }
      return attributeToObj;
    }));
  }

  getInternalExternalAttributeToData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/InternalExternalAttributeTo",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res:any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      const internalExternalAttributeToObj = {};
      for (const row of res) {
        internalExternalAttributeToObj[row.internalExternalAttributeToId] = row.internalExternalAttributeToName;
      }
      return internalExternalAttributeToObj;
    }));
  }

  getProductData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/Product",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      const productObj = {};
      for (const row of res) {
        productObj[row.productId] = row.productName;
      }
      return productObj;
    }));
  }

  getGESGroupProductMappingData() {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/GESGroupProductMapping",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res:any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return;
      }
      
      const gesGroupProductMappingObj = {};
      for (const row of res) {
        const groupId = row.gesGroupId;
        const productId = row.productId;
        if (gesGroupProductMappingObj.hasOwnProperty(groupId)) {
          gesGroupProductMappingObj[groupId].push(productId);
        } else {
          gesGroupProductMappingObj[groupId] = [productId];
        }
      }
      return gesGroupProductMappingObj;
    }));
  }

  postOttrMonitoringData(data: OttrMonitoring) {
    var addOttrMonitoringData = JSON.stringify(data);
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/OttrMonitoring/create",
      "data": addOttrMonitoringData,
      "requestType": "POST"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res:any) => {
      var resdata = JSON.parse(res.data);
      if(resdata.code !=undefined && resdata.code!=null && res.code!="" &&  resdata.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(resdata);
        return;
      }
      else
      {
        return res;
      }
    }));
  }

  putOttrMonitoringData(data) {
    const updateOttrMonitoringData = JSON.stringify(data);
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/OttrMonitoring/update",
      "data": updateOttrMonitoringData,
      "requestType": "POST"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res:any) => {
      var resdata = JSON.parse(res.data);
      if(resdata.code !=undefined && resdata.code!=null && res.code!="" &&  resdata.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(resdata);
        return;
      }
      else
      {
        return res;
      }
    }));
  }

  logout() {
    const requstProcessorData = {
      "app": "Security",
      "action": "/api/UserSession/logout",
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {

      return res;
    }));
  }

  triggerExportData(data) {
    this.exportData.next(data);
  }

  downloadOttrData(gesGroupId,filterParams) {
    const filterParamData = JSON.stringify(filterParams);
    const requestProcessorData = {
      "app": "OTTR",
      "action": "/api/OttrMonitoring/Download/" + gesGroupId+"/"+this.getUserCountry(),
      "data": filterParamData,
      "requestType": "POSTDOWNLOAD"
    }
    return this.http.post(this.fileProcessorUrl, requestProcessorData, {
      observe: 'response',
      responseType: 'blob'
    });
  }

  uploadFiles(files: any) {
    const formData = new FormData();
    _.forEach(files, (file, index) => {
      formData.append('Files', file);
    });


    formData.append('app', 'OTTR');
    formData.append('action', '/api/FileUpload/Upload/'+this.getSelectedGESLOB());
    formData.append('requestType', 'POST');

    return this.http.post<any>(this.fileProcessorUrl, formData).
    pipe(map((res:any) => {
      var resdata = JSON.parse(res.data);
      if(resdata.code !=undefined && resdata.code!=null && resdata.code!="" &&  resdata.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(resdata);
        return;
      }
      else
      {
        return res;
      }
    }));

  }

  getEmployeeNameFromEIDPeopleSoftAPI(eid)
  {
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/UserPeopleSoft/"+eid,
      "requestType": "GET"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res: any) => {
      if(res.code !=undefined && res.code!=null && res.code!="" &&  res.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(res);
        return null;
      }     
      return res;
    }));   
  }


  

  triggerGesGroupChangeEvent(gesGroupId) {
    this.gesGroupChangeEvent.next(gesGroupId);
  }
  triggerSurveyRequestChangeEvent(params) {
    this.surveyRequestChangeEvent.next(params);
  }
  triggerUploadExcelEvent(params) {    
    this.uploadExcelEvent.next(params);
  }
  triggerUnauthorizedAccessEvent(params)
  {
    this.unauthorizedAccess.next(params);
  }
  triggerFilterClearEvent(params){
    this.filterClearEvent.next(params);
  }    


  deleteOttrMonitoringData(data:any) {
    var deleteOttrMonitoringData = JSON.stringify(data);
    const requstProcessorData = {
      "app": "OTTR",
      "action": "/api/OttrMonitoring/delete",
      "data": deleteOttrMonitoringData,
      "requestType": "POST"
    }
    return this.http.post(this.url, requstProcessorData).pipe(map((res:any) => {
      var resdata = JSON.parse(res.data);
      if(resdata.code !=undefined && resdata.code!=null && res.code!="" &&  resdata.code == 401)
      {
        this.triggerUnauthorizedAccessEvent(resdata);
        return;
      }
      else
      {
        return res;
      }
    }));
  }
}