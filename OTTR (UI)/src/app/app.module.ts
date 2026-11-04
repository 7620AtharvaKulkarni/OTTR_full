import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AgGridModule } from 'ag-grid-angular';
import { AppRoutingModule } from './app-routing.module';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { ToastrModule } from 'ngx-toastr';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { NgxSpinnerModule } from 'ngx-spinner';
import { NgxSpinnerService } from 'ngx-spinner';

import { AppComponent } from './app.component';
import { OttrListComponent } from './components/ottr-list/ottr-list.component';
import { OttrService } from './services/ottr.service';
import { DropdownComponent } from './components/dropdown/dropdown.component';
import { CalenderComponent } from './components/calender/calender.component';
import { TextareaComponent } from './components/textarea/textarea.component';
import { ButtonRendererComponent } from './renderer/button-renderer/button-renderer.component';
import { CustomHeaderComponent } from './components/custom-header/custom-header.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppHttpInterceptor } from './app-http-interceptor';
import { CookieService } from 'ngx-cookie-service';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatMenuModule } from '@angular/material/menu';
import { MatSelectModule } from '@angular/material/select';
import { FooterComponent } from './footer/footer.component';
import { ToastermessageComponent } from './toastermessage/toastermessage.component';
import { UploadDialogComponent } from './upload-dialog-box/upload-dialog.component';
import { FileSaverModule } from 'ngx-filesaver';
import { UsageTrackerService } from './services/usage-tracker.service';
import { HeaderDeleteComponent } from './components/header-delete/header-delete.component';
import { ConfirmationModalComponent } from './components/confirmation-modal/confirmation-modal.component';
@NgModule({
  declarations: [
    AppComponent,
    OttrListComponent,
    DropdownComponent,
    CalenderComponent,
    TextareaComponent,
    UploadDialogComponent,
    ButtonRendererComponent,
    CustomHeaderComponent,
    FooterComponent,
    ToastermessageComponent,    
    HeaderDeleteComponent,
    ConfirmationModalComponent 
  ],
  imports: [
    BrowserModule,
    MatSelectModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    AgGridModule.withComponents([DropdownComponent,HeaderDeleteComponent]),
    BrowserAnimationsModule,
    BrowserModule,
    MaterialModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatMenuModule,
    FileSaverModule,
    NgxSpinnerModule,
    ToastrModule.forRoot({
      toastComponent: ToastermessageComponent, // added custom toast!
      maxOpened: 1,
      preventDuplicates: true
    })
  ],
  providers: [OttrService,
    { provide: HTTP_INTERCEPTORS, useClass: AppHttpInterceptor, multi: true }, 
    CookieService,
    UsageTrackerService,
    {provide: LocationStrategy, useClass: HashLocationStrategy},
    NgxSpinnerService],
    entryComponents: [
    UploadDialogComponent,
    ConfirmationModalComponent
   
  ],
  bootstrap: [AppComponent],
  
})
export class AppModule { }
