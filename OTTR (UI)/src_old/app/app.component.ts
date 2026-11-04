import { Component, ViewChild, OnInit } from '@angular/core';
import { OttrService } from './services/ottr.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatMenuTrigger } from '@angular/material/menu';
import * as _ from 'lodash';
import { UploadDialogComponent } from './upload-dialog-box/upload-dialog.component';
import { environment } from 'src/environments/environment';
import { NgxSpinnerService } from "ngx-spinner";
import { UsageTrackerService } from './services/usage-tracker.service';
import { FileSaverService } from 'ngx-filesaver';


@Component({
	selector: 'app-root',
	templateUrl: './app.component.html',
	styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
	uploadExcel: string;
	userId: string;
	userInitial: string;
	userName: string;
	buisnessTitle: string;
	addRecord: string;
	disableFilter:boolean=true;
	isDisable: boolean;
	selectedGESGroup = "-1";
	GESGroups = [];
	EditRole: boolean;
	AdminRole: boolean;
	roleList: any = {};
	isGridEmpty = true;
	allowAllOption = true;
	uploadDialogRef: MatDialogRef<UploadDialogComponent>;
	version: any;
	@ViewChild(MatMenuTrigger, { static: false }) trigger: MatMenuTrigger;
	GESGroupName: any;
	userCountry: any;
	isIEOrEdge: boolean;
	helpFileName: any;
	feedbackLink: string;
	sample: any;
	defectFormLink: any;
	openMyMenu() {
		this.trigger.toggleMenu();
	}
	closeMyMenu() {
		this.trigger.closeMenu();
	}

	ngOnInit(): void {
	}

	constructor(private ottrService: OttrService,
		public usageTrackerService: UsageTrackerService,
		public dialog: MatDialog,
		private toaster: ToastrService,
		private _FileSaverService: FileSaverService,
		private spinner: NgxSpinnerService) {
		this.EditRole = false;
		this.AdminRole = false;
		this.isIEOrEdge = /msie\s|trident\/|edge\//i.test(window.navigator.userAgent);
		this.userId = "@userId";
		
		// if (this.ottrService.cookieValue != null && this.ottrService.cookieValue != '') {
		// 	this.userId = this.ottrService.cookieValue;
		// } else {
		// 	this.userId = null;
		// }
		this.getUserDetails(this.userId);
		this.spinner.show();
		setTimeout(() => {
			this.spinner.hide();
		}, 5000);

		//this.testApi();

		this.ottrService.unauthorizedAccess.subscribe(data=>{
			this.spinner.hide();
			console.log(data);
			if(data!=null && data!=undefined && data!="")
			{
				if(data.data.toLowerCase().indexOf("unauthorized")>-1 && data.code == 401)
				{
					this.toaster.error(data.data,'Error');
					// setTimeout(()=>{
					// 	window.location.href = environment.signinUrl;
					// },1000);
				}				
			}			
		});

	}


	


	getGESGroups() {
		this.ottrService.getGESGroupForSelection().subscribe(data => {
			if (this.AdminRole) {
				this.GESGroups = data;
			}
			else {
				for (var item of data) {
					if (item.gesGroupName.toLowerCase().indexOf(this.ottrService.getUserCountry().toLowerCase()) > -1) {
						this.GESGroups.push(item);
					}
				}
			}
		});
	}

	onSelectGESGroup() {
		this.spinner.show();
		if (this.selectedGESGroup == "0") {
			this.ottrService.setAllOptionSelected(true);
		}
		else {
			this.ottrService.setAllOptionSelected(false);
		}
		if (this.selectedGESGroup == "-1" || this.selectedGESGroup == "0") {
			this.isDisable = true;
			this.spinner.hide();
		}
		else {
			this.isDisable = false;
		}
		setTimeout(() => {
			if (this.selectedGESGroup != "0" && this.selectedGESGroup != "-1") {
				this.GESGroupName = this.GESGroups.filter(x => x.gesGroupId == this.selectedGESGroup)[0].gesGroupName;
				this.ottrService.setSelectedGESLOB(this.GESGroupName);
			}
			else {
				this.GESGroupName = "All";
			}
		}, 200);
		setTimeout(() => {
			this.ottrService.triggerGesGroupChangeEvent(this.selectedGESGroup);
		}, 1000)
	}

	gridPopulateEvent(event) {
		this.isGridEmpty = event;
	}

	disableFilterButton(event)
	{
		this.disableFilter = event;
	}

	getUserDetails(userId) {
		this.ottrService.getUserDetails(userId).subscribe((res: any) => {
			console.log("res", res);
			if ((res.Status === 403 || res.Status === 401) && res.data === "unauthorized access") {
				this.toaster.error('You are not authorized to access this application', 'Error');
				setTimeout(() => {
					window.location.href = res.URL;
				}, 1000);
			} else {
				this.EditRole = res.listOfRoleName.includes(environment.EditRoleName);
				this.AdminRole = res.listOfRoleName.includes(environment.AdminRoleName);
				this.ottrService.roleList = res.listOfRoleName;				
				console.log(this.ottrService.roleList);
				this.isDisable = true;
				//this.isDisable = !this.EditRole;
				if (_.includes(res.UserApplicationList, environment.appName)) {
					this.userInitial = (res.employee.nameFirst).charAt(0) + (res.employee.nameLast).charAt(0);
					this.userName = res.employee.nameFirst + ' ' + res.employee.nameLast;
					this.buisnessTitle = res.employee.businessTitle;
					this.ottrService.setUserCountry(res.employee.country);
					this.userCountry = res.employee.country;
					this.getGESGroups();
					this.getConfigurationData();
				} else {
					this.toaster.error('You are not authorized to access this application', 'Error');
					setTimeout(() => {
						window.location.href = res.redirectUrl;
					}, 1000);
				}
			}
		}, err => {
			this.toaster.error('Failed to get User details', 'Error');
		});
	}

	/* ***LOG OUT USER*** */
	logOut() {
		this.ottrService.logout().subscribe(res => {
			if (res.status == 200) {
				this.toaster.success('Successfully Log out', 'Done');
				window.location.href = res.URL;
			} else {
				this.toaster.error(res.data, 'Error');
			}
		}, err => { })

	}

	addNewRecord() {
		this.addRecord = "add" + Math.random();
		this.isDisable = true;
	}

	exportToExcel() {
		this.spinner.show();
		this.ottrService.triggerExportData("export" + Math.random());
	}


	Uploads() {
		if (this.selectedGESGroup == null || this.selectedGESGroup == undefined || this.selectedGESGroup == "0" || this.selectedGESGroup == "-1") {
			this.toaster.error("Please select GES LOB to enable upload of the data", 'Error');
			return;
		}
		this.uploadDialogRef = this.dialog.open(UploadDialogComponent, {
			disableClose: true,
			height: '40vh',
			width: '34vw'
		});
		this.uploadDialogRef.afterClosed().subscribe(result => {
			if (result.status == true) {
				this.ottrService.triggerUploadExcelEvent("test");
			}
		});
	}

	ClearFilters(){
		this.ottrService.triggerFilterClearEvent(true);
	}

	disableCreateButton(event) {		
		if (this.EditRole && this.selectedGESGroup != "0" && this.selectedGESGroup != "-1") {
			this.isDisable = event;
		}
	}
	

	getConfigurationData() {
		this.usageTrackerService.getConfigurationDataForUI().subscribe(data => {

			this.version = JSON.parse(data.fileData.version)[0].version;
			this.feedbackLink = JSON.parse(data.fileData['feedback-link'])[0].link;
			this.defectFormLink = JSON.parse(data.fileData['feedback-link'])[1].link;
			this.helpFileName = data.downloadableFileNames.help;
			this.sample = data.downloadableFileNames.Sample;
		}, err => { });
	}

	downloadHelpFile() {
		if (this.helpFileName != null && this.helpFileName != "" && this.helpFileName != undefined) {
			this.usageTrackerService.downloadFileFromUsageTracker(this.helpFileName).subscribe(data => {
				if (this.isIEOrEdge == true) {
					var blob = new Blob([data], { type: 'application/pdf' });
					window.navigator.msSaveBlob(blob, this.helpFileName);
				}
				else {
					var blob = new Blob([data], { type: 'application/pdf' });					
					this._FileSaverService.save(blob,this.helpFileName);
					// var blob = new Blob([data], { type: 'application/pdf' });
					// const urlCreator = window.URL;
					// const url = urlCreator.createObjectURL(blob);
					// const a = document.createElement('a');
					// document.body.appendChild(a);
					// a.href = url;
					// a.download = this.helpFileName;
					// a.click();
					// a.remove();
					// window.URL.revokeObjectURL(url);
				}
			}, err => {

			});

		}

	}


	downloadTemplate() {
		if (this.sample != null && this.sample != "" && this.sample != undefined) {
			this.usageTrackerService.downloadFileFromUsageTracker(this.sample).subscribe(data => {
				if (this.isIEOrEdge == true) {
					var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
					window.navigator.msSaveBlob(blob, this.sample);
				}
				else {
					var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
					this._FileSaverService.save(blob,this.sample);
					// const urlCreator = window.URL;
					// const url = urlCreator.createObjectURL(blob);
					// const a = document.createElement('a');
					// document.body.appendChild(a);
					// a.href = url;
					// a.download = "Sample.xlsx";
					// a.click();
					// a.remove();
					// window.URL.revokeObjectURL(url);
				}
			}, err => {

			});

		}

	}





	openFeedbackForm() {
		window.open(this.feedbackLink, "_blank");
	}

	openDefectForm(){
		window.open(this.defectFormLink,"_blank");
	}




}
