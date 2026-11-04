import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable({
    providedIn: 'root'
})

export class UsageTrackerService {
    public configurationData: any;

    constructor(
        private http: HttpClient
    ) { }

    getConfigurationData() {
        return this.configurationData;
    }

    setConfigurationData(configData: any) {
        this.configurationData = configData;
    }


    getConfigurationDataForUI() {
        const requstProcessorData = {
            app: environment.usageApplicationName,
            action: environment.applicationFilesUrl + '/' + environment.appName,
            requestType: 'GET'
        };
        return this.http.post<any>(environment.apiUrl, requstProcessorData)
            .pipe(map(res => res));
    }


    downloadFileFromUsageTracker(filename): Observable<Blob> {
        const requstProcessorData = {
            app: environment.usageApplicationName,
            action: environment.downloadApplicationFiles + '/' + environment.appName + '/' + filename,
            requestType: 'GET',
        };
        const httpOptions = {
            responseType: 'blob' as 'json',
        };
        return this.http.post<any>(environment.fileProcessorUrl, requstProcessorData, httpOptions)
            .pipe(map(res => res));
    }


}  