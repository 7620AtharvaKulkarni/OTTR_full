export class OttrMonitoring {
    constructor () {
        this.ottrId = null;
        this.gespmEID = null;
        this.gespm = null;
        this.projectId = null;
        this.projectName = null;
        this.deliverables = null;
        this.surveyRequest = "NA";
        this.deliveryHours = null;
        this.reworkHours = null;
        this.surveyType = "";
        this.month = null;
        this.baseDate = null;
        this.planDate = null;
        this.actualDate = null;
        this.gesGroupId = null;
        this.regionId = null;
        this.productId = null;
        this.attributeToId = null;
        this.comments = null;
        this.customerSurvey = '';
        this.fopmName = null;
        this.fopmId = null;
        this.selectedRow=false;
        this.customerSurveyTemplate = '';
        this.spendPlan_Pole = null;
        this.spendPlan_Country = null;
        this.spendPlan_Region = null;
        this.internalExternalAttributeId = null;
        this.trackingToDesignInput = null;
        this.gesResourceId = null;
        this.gesResourceName = null;
    }

    public ottrId: number;
    public gespmEID:string;
    public internalExternalAttributeId:string;
    public trackingToDesignInput:string;
 
    public surveyRequest: string;
    public deliveryHours: number;
    public reworkHours: number;
    public surveyType: string;
    public gespm: string;
    public gesResourceId:string;
    public gesResourceName:string;

    public projectId: string;

    public projectName: string;

    public deliverables: string;

    public month: string;

    public baseDate: Date;

    public planDate: Date;

    public actualDate: Date;

    public gesGroupId: number;

    public regionId: number;

    public productId: number;

    public attributeToId: number;

    public comments: string;

    public customerSurvey: string;

    public fopmName: string;

    public fopmId: string;

    public customerSurveyTemplate: string;

    public isActive: boolean;
    public selectedRow: boolean;

    public createdBy: string;

    public createdOn: Date

    public modifiedBy: string;

    public modifiedOn: Date;
    public spendPlan_Pole:number;
    public spendPlan_Region:number;
    public spendPlan_Country:number;
    public spendPlan_PoleId:number;
    public spendPlan_RegionId:number;
    public spendPlan_CountryId:number;
}