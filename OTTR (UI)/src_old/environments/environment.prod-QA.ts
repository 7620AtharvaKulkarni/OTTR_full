export const environment = {
  production: true,
  apiUrl: 'https://geshpsappsuiteqa.honeywell.com/GESExcellence/SecurityManager/PlatformUIManager/RequestProcessor',
  fileProcessorUrl: 'https://geshpsappsuiteqa.honeywell.com/GESExcellence/SecurityManager/PlatformUIManager/FileProcessor',
  appName: 'OTTR',
  EditRoleName: 'OTTR-Staff',
  AdminRoleName: 'OTTR-Admin',
  acceptFile: '.xls,.xlsx',
  usageApplicationName: 'usageTracker',
  applicationFilesUrl: '/api/ApplicationFiles/GetApplicationFiles',
  downloadApplicationFiles: '/api/ApplicationFiles/ApplicationFiles/DownloadFile',
  signinUrl:'https://qcwa.honeywell.com/sp/startSSO.ping?PartnerIdPId=idp:standard&RequestedAuthnCtx=45520&TargetResource=https%3A%2F%2Fssappsqa.honeywell.com%2FGESExcellence%2FSecurityManager%2FPlatformUIManager%2FAccount%2FHome'
};
