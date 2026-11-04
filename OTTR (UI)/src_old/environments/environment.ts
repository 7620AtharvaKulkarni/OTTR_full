// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  apiUrl: 'http://localhost:9002/RequestProcessor',
  fileProcessorUrl: 'http://localhost:9002/FileProcessor',
  appName: 'OTTR',
  EditRoleName:'OTTR-Staff',
  AdminRoleName:'OTTR-Admin',
  acceptFile: '.xls,.xlsx',
  usageApplicationName:'usageTracker',
  applicationFilesUrl:'/api/ApplicationFiles/GetApplicationFiles',
  downloadApplicationFiles:'/api/ApplicationFiles/ApplicationFiles/DownloadFile',
  signinUrl:'https://qcwa.honeywell.com/sp/startSSO.ping?PartnerIdPId=idp:standard&RequestedAuthnCtx=45520&TargetResource=https%3A%2F%2Fssappsqa.honeywell.com%2FGESExcellence%2FSecurityManager%2FPlatformUIManager%2FAccount%2FHome'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
