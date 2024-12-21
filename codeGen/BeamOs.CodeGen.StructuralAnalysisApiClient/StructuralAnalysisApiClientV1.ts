//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.2.0.0 (NJsonSchema v11.1.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

export interface IStructuralAnalysisApiClientV1 {

    /**
     * @return OK
     */
    createNode(modelId: string, body: CreateNodeRequest): Promise<ResultOfNodeResponse>;

    /**
     * @return OK
     */
    updateNode(modelId: string, body: UpdateNodeRequest): Promise<ResultOfNodeResponse>;

    /**
     * @param id (optional) 
     * @return OK
     */
    createModel(name: string, description: string, id: string | undefined, body: PhysicalModelSettings): Promise<ResultOfModelResponse2>;
}

export class StructuralAnalysisApiClientV1 implements IStructuralAnalysisApiClientV1 {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "https://localhost:7060";
    }

    /**
     * @return OK
     */
    createNode(modelId: string, body: CreateNodeRequest): Promise<ResultOfNodeResponse> {
        let url_ = this.baseUrl + "/api/models/{modelId}/nodes";
        if (modelId === undefined || modelId === null)
            throw new Error("The parameter 'modelId' must be defined.");
        url_ = url_.replace("{modelId}", encodeURIComponent("" + modelId));
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(body);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processCreateNode(_response);
        });
    }

    protected processCreateNode(response: Response): Promise<ResultOfNodeResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ResultOfNodeResponse.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ResultOfNodeResponse>(null as any);
    }

    /**
     * @return OK
     */
    updateNode(modelId: string, body: UpdateNodeRequest): Promise<ResultOfNodeResponse> {
        let url_ = this.baseUrl + "/api/models/{modelId}/nodes";
        if (modelId === undefined || modelId === null)
            throw new Error("The parameter 'modelId' must be defined.");
        url_ = url_.replace("{modelId}", encodeURIComponent("" + modelId));
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(body);

        let options_: RequestInit = {
            body: content_,
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processUpdateNode(_response);
        });
    }

    protected processUpdateNode(response: Response): Promise<ResultOfNodeResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ResultOfNodeResponse.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ResultOfNodeResponse>(null as any);
    }

    /**
     * @param id (optional) 
     * @return OK
     */
    createModel(name: string, description: string, id: string | undefined, body: PhysicalModelSettings): Promise<ResultOfModelResponse2> {
        let url_ = this.baseUrl + "/api/models?";
        if (name === undefined || name === null)
            throw new Error("The parameter 'name' must be defined and cannot be null.");
        else
            url_ += "Name=" + encodeURIComponent("" + name) + "&";
        if (description === undefined || description === null)
            throw new Error("The parameter 'description' must be defined and cannot be null.");
        else
            url_ += "Description=" + encodeURIComponent("" + description) + "&";
        if (id === null)
            throw new Error("The parameter 'id' cannot be null.");
        else if (id !== undefined)
            url_ += "Id=" + encodeURIComponent("" + id) + "&";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(body);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processCreateModel(_response);
        });
    }

    protected processCreateModel(response: Response): Promise<ResultOfModelResponse2> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ResultOfModelResponse2.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ResultOfModelResponse2>(null as any);
    }
}

export class AnalysisSettingsContract implements IAnalysisSettingsContract {
    element1DAnalysisType?: number;

    [key: string]: any;

    constructor(data?: IAnalysisSettingsContract) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.element1DAnalysisType = _data["element1DAnalysisType"];
        }
    }

    static fromJS(data: any): AnalysisSettingsContract {
        data = typeof data === 'object' ? data : {};
        let result = new AnalysisSettingsContract();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["element1DAnalysisType"] = this.element1DAnalysisType;
        return data;
    }
}

export interface IAnalysisSettingsContract {
    element1DAnalysisType?: number;

    [key: string]: any;
}

export class CreateNodeRequest implements ICreateNodeRequest {
    locationPoint!: Point;
    restraint!: NullableOfRestraint | undefined;
    id?: number | undefined;

    [key: string]: any;

    constructor(data?: ICreateNodeRequest) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.locationPoint = new Point();
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.locationPoint = _data["locationPoint"] ? Point.fromJS(_data["locationPoint"]) : new Point();
            this.restraint = _data["restraint"] ? NullableOfRestraint.fromJS(_data["restraint"]) : <any>undefined;
            this.id = _data["id"];
        }
    }

    static fromJS(data: any): CreateNodeRequest {
        data = typeof data === 'object' ? data : {};
        let result = new CreateNodeRequest();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["locationPoint"] = this.locationPoint ? this.locationPoint.toJSON() : <any>undefined;
        data["restraint"] = this.restraint ? this.restraint.toJSON() : <any>undefined;
        data["id"] = this.id;
        return data;
    }
}

export interface ICreateNodeRequest {
    locationPoint: Point;
    restraint: NullableOfRestraint | undefined;
    id?: number | undefined;

    [key: string]: any;
}

export class Exception implements IException {
    data?: any | undefined;
    helpLink?: string | undefined;
    hResult?: number;
    innerException?: Exception | undefined;
    message?: string | undefined;
    source?: string | undefined;
    stackTrace?: string | undefined;
    targetSite?: MethodBase | undefined;

    [key: string]: any;

    constructor(data?: IException) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.data = _data["data"];
            this.helpLink = _data["helpLink"];
            this.hResult = _data["hResult"];
            this.innerException = _data["innerException"] ? Exception.fromJS(_data["innerException"]) : <any>undefined;
            this.message = _data["message"];
            this.source = _data["source"];
            this.stackTrace = _data["stackTrace"];
            this.targetSite = _data["targetSite"] ? MethodBase.fromJS(_data["targetSite"]) : <any>undefined;
        }
    }

    static fromJS(data: any): Exception {
        data = typeof data === 'object' ? data : {};
        let result = new Exception();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["data"] = this.data;
        data["helpLink"] = this.helpLink;
        data["hResult"] = this.hResult;
        data["innerException"] = this.innerException ? this.innerException.toJSON() : <any>undefined;
        data["message"] = this.message;
        data["source"] = this.source;
        data["stackTrace"] = this.stackTrace;
        data["targetSite"] = this.targetSite ? this.targetSite.toJSON() : <any>undefined;
        return data;
    }
}

export interface IException {
    data?: any | undefined;
    helpLink?: string | undefined;
    hResult?: number;
    innerException?: Exception | undefined;
    message?: string | undefined;
    source?: string | undefined;
    stackTrace?: string | undefined;
    targetSite?: MethodBase | undefined;

    [key: string]: any;
}

export class MethodBase implements IMethodBase {

    [key: string]: any;

    constructor(data?: IMethodBase) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
        }
    }

    static fromJS(data: any): MethodBase {
        data = typeof data === 'object' ? data : {};
        let result = new MethodBase();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        return data;
    }
}

export interface IMethodBase {

    [key: string]: any;
}

export class ModelResponse implements IModelResponse {
    id!: string;
    name!: string;
    description!: string;
    settings!: PhysicalModelSettings;
    nodes?: NodeResponse3[] | undefined;

    [key: string]: any;

    constructor(data?: IModelResponse) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.settings = new PhysicalModelSettings();
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.id = _data["id"];
            this.name = _data["name"];
            this.description = _data["description"];
            this.settings = _data["settings"] ? PhysicalModelSettings.fromJS(_data["settings"]) : new PhysicalModelSettings();
            if (Array.isArray(_data["nodes"])) {
                this.nodes = [] as any;
                for (let item of _data["nodes"])
                    this.nodes!.push(NodeResponse3.fromJS(item));
            }
        }
    }

    static fromJS(data: any): ModelResponse {
        data = typeof data === 'object' ? data : {};
        let result = new ModelResponse();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["id"] = this.id;
        data["name"] = this.name;
        data["description"] = this.description;
        data["settings"] = this.settings ? this.settings.toJSON() : <any>undefined;
        if (Array.isArray(this.nodes)) {
            data["nodes"] = [];
            for (let item of this.nodes)
                data["nodes"].push(item.toJSON());
        }
        return data;
    }
}

export interface IModelResponse {
    id: string;
    name: string;
    description: string;
    settings: PhysicalModelSettings;
    nodes?: NodeResponse3[] | undefined;

    [key: string]: any;
}

export class NodeResponse implements INodeResponse {
    id!: number;
    modelId!: string;
    locationPoint!: Point;
    restraint!: Restraint;

    [key: string]: any;

    constructor(data?: INodeResponse) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.locationPoint = new Point();
            this.restraint = new Restraint();
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.id = _data["id"];
            this.modelId = _data["modelId"];
            this.locationPoint = _data["locationPoint"] ? Point.fromJS(_data["locationPoint"]) : new Point();
            this.restraint = _data["restraint"] ? Restraint.fromJS(_data["restraint"]) : new Restraint();
        }
    }

    static fromJS(data: any): NodeResponse {
        data = typeof data === 'object' ? data : {};
        let result = new NodeResponse();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["id"] = this.id;
        data["modelId"] = this.modelId;
        data["locationPoint"] = this.locationPoint ? this.locationPoint.toJSON() : <any>undefined;
        data["restraint"] = this.restraint ? this.restraint.toJSON() : <any>undefined;
        return data;
    }
}

export interface INodeResponse {
    id: number;
    modelId: string;
    locationPoint: Point;
    restraint: Restraint;

    [key: string]: any;
}

export class NodeResponse3 implements INodeResponse3 {
    id!: number;
    modelId!: string;
    locationPoint!: Point;
    restraint!: Restraint;

    [key: string]: any;

    constructor(data?: INodeResponse3) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.locationPoint = new Point();
            this.restraint = new Restraint();
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.id = _data["id"];
            this.modelId = _data["modelId"];
            this.locationPoint = _data["locationPoint"] ? Point.fromJS(_data["locationPoint"]) : new Point();
            this.restraint = _data["restraint"] ? Restraint.fromJS(_data["restraint"]) : new Restraint();
        }
    }

    static fromJS(data: any): NodeResponse3 {
        data = typeof data === 'object' ? data : {};
        let result = new NodeResponse3();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["id"] = this.id;
        data["modelId"] = this.modelId;
        data["locationPoint"] = this.locationPoint ? this.locationPoint.toJSON() : <any>undefined;
        data["restraint"] = this.restraint ? this.restraint.toJSON() : <any>undefined;
        return data;
    }
}

export interface INodeResponse3 {
    id: number;
    modelId: string;
    locationPoint: Point;
    restraint: Restraint;

    [key: string]: any;
}

export class NullableOfPartialPoint implements INullableOfPartialPoint {
    x?: number | undefined;
    y?: number | undefined;
    z?: number | undefined;
    lengthUnit!: number;

    [key: string]: any;

    constructor(data?: INullableOfPartialPoint) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.x = _data["x"];
            this.y = _data["y"];
            this.z = _data["z"];
            this.lengthUnit = _data["lengthUnit"];
        }
    }

    static fromJS(data: any): NullableOfPartialPoint {
        data = typeof data === 'object' ? data : {};
        let result = new NullableOfPartialPoint();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["x"] = this.x;
        data["y"] = this.y;
        data["z"] = this.z;
        data["lengthUnit"] = this.lengthUnit;
        return data;
    }
}

export interface INullableOfPartialPoint {
    x?: number | undefined;
    y?: number | undefined;
    z?: number | undefined;
    lengthUnit: number;

    [key: string]: any;
}

export class NullableOfPartialRestraint implements INullableOfPartialRestraint {
    canTranslateAlongX?: boolean | undefined;
    canTranslateAlongY?: boolean | undefined;
    canTranslateAlongZ?: boolean | undefined;
    canRotateAboutX?: boolean | undefined;
    canRotateAboutY?: boolean | undefined;
    canRotateAboutZ?: boolean | undefined;

    [key: string]: any;

    constructor(data?: INullableOfPartialRestraint) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.canTranslateAlongX = _data["canTranslateAlongX"];
            this.canTranslateAlongY = _data["canTranslateAlongY"];
            this.canTranslateAlongZ = _data["canTranslateAlongZ"];
            this.canRotateAboutX = _data["canRotateAboutX"];
            this.canRotateAboutY = _data["canRotateAboutY"];
            this.canRotateAboutZ = _data["canRotateAboutZ"];
        }
    }

    static fromJS(data: any): NullableOfPartialRestraint {
        data = typeof data === 'object' ? data : {};
        let result = new NullableOfPartialRestraint();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["canTranslateAlongX"] = this.canTranslateAlongX;
        data["canTranslateAlongY"] = this.canTranslateAlongY;
        data["canTranslateAlongZ"] = this.canTranslateAlongZ;
        data["canRotateAboutX"] = this.canRotateAboutX;
        data["canRotateAboutY"] = this.canRotateAboutY;
        data["canRotateAboutZ"] = this.canRotateAboutZ;
        return data;
    }
}

export interface INullableOfPartialRestraint {
    canTranslateAlongX?: boolean | undefined;
    canTranslateAlongY?: boolean | undefined;
    canTranslateAlongZ?: boolean | undefined;
    canRotateAboutX?: boolean | undefined;
    canRotateAboutY?: boolean | undefined;
    canRotateAboutZ?: boolean | undefined;

    [key: string]: any;
}

export class NullableOfRestraint implements INullableOfRestraint {
    canTranslateAlongX!: boolean;
    canTranslateAlongY!: boolean;
    canTranslateAlongZ!: boolean;
    canRotateAboutX!: boolean;
    canRotateAboutY!: boolean;
    canRotateAboutZ!: boolean;

    [key: string]: any;

    constructor(data?: INullableOfRestraint) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.canTranslateAlongX = _data["canTranslateAlongX"];
            this.canTranslateAlongY = _data["canTranslateAlongY"];
            this.canTranslateAlongZ = _data["canTranslateAlongZ"];
            this.canRotateAboutX = _data["canRotateAboutX"];
            this.canRotateAboutY = _data["canRotateAboutY"];
            this.canRotateAboutZ = _data["canRotateAboutZ"];
        }
    }

    static fromJS(data: any): NullableOfRestraint {
        data = typeof data === 'object' ? data : {};
        let result = new NullableOfRestraint();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["canTranslateAlongX"] = this.canTranslateAlongX;
        data["canTranslateAlongY"] = this.canTranslateAlongY;
        data["canTranslateAlongZ"] = this.canTranslateAlongZ;
        data["canRotateAboutX"] = this.canRotateAboutX;
        data["canRotateAboutY"] = this.canRotateAboutY;
        data["canRotateAboutZ"] = this.canRotateAboutZ;
        return data;
    }
}

export interface INullableOfRestraint {
    canTranslateAlongX: boolean;
    canTranslateAlongY: boolean;
    canTranslateAlongZ: boolean;
    canRotateAboutX: boolean;
    canRotateAboutY: boolean;
    canRotateAboutZ: boolean;

    [key: string]: any;
}

export class PhysicalModelSettings implements IPhysicalModelSettings {
    unitSettings!: UnitSettingsContract;
    analysisSettings?: AnalysisSettingsContract | undefined;
    yAxisUp?: boolean;

    [key: string]: any;

    constructor(data?: IPhysicalModelSettings) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
        if (!data) {
            this.unitSettings = new UnitSettingsContract();
            this.yAxisUp = true;
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.unitSettings = _data["unitSettings"] ? UnitSettingsContract.fromJS(_data["unitSettings"]) : new UnitSettingsContract();
            this.analysisSettings = _data["analysisSettings"] ? AnalysisSettingsContract.fromJS(_data["analysisSettings"]) : <any>undefined;
            this.yAxisUp = _data["yAxisUp"] !== undefined ? _data["yAxisUp"] : true;
        }
    }

    static fromJS(data: any): PhysicalModelSettings {
        data = typeof data === 'object' ? data : {};
        let result = new PhysicalModelSettings();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["unitSettings"] = this.unitSettings ? this.unitSettings.toJSON() : <any>undefined;
        data["analysisSettings"] = this.analysisSettings ? this.analysisSettings.toJSON() : <any>undefined;
        data["yAxisUp"] = this.yAxisUp;
        return data;
    }
}

export interface IPhysicalModelSettings {
    unitSettings: UnitSettingsContract;
    analysisSettings?: AnalysisSettingsContract | undefined;
    yAxisUp?: boolean;

    [key: string]: any;
}

export class Point implements IPoint {
    x!: number;
    y!: number;
    z!: number;
    lengthUnit!: number;

    [key: string]: any;

    constructor(data?: IPoint) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.x = _data["x"];
            this.y = _data["y"];
            this.z = _data["z"];
            this.lengthUnit = _data["lengthUnit"];
        }
    }

    static fromJS(data: any): Point {
        data = typeof data === 'object' ? data : {};
        let result = new Point();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["x"] = this.x;
        data["y"] = this.y;
        data["z"] = this.z;
        data["lengthUnit"] = this.lengthUnit;
        return data;
    }
}

export interface IPoint {
    x: number;
    y: number;
    z: number;
    lengthUnit: number;

    [key: string]: any;
}

export class Restraint implements IRestraint {
    canTranslateAlongX!: boolean;
    canTranslateAlongY!: boolean;
    canTranslateAlongZ!: boolean;
    canRotateAboutX!: boolean;
    canRotateAboutY!: boolean;
    canRotateAboutZ!: boolean;

    [key: string]: any;

    constructor(data?: IRestraint) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.canTranslateAlongX = _data["canTranslateAlongX"];
            this.canTranslateAlongY = _data["canTranslateAlongY"];
            this.canTranslateAlongZ = _data["canTranslateAlongZ"];
            this.canRotateAboutX = _data["canRotateAboutX"];
            this.canRotateAboutY = _data["canRotateAboutY"];
            this.canRotateAboutZ = _data["canRotateAboutZ"];
        }
    }

    static fromJS(data: any): Restraint {
        data = typeof data === 'object' ? data : {};
        let result = new Restraint();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["canTranslateAlongX"] = this.canTranslateAlongX;
        data["canTranslateAlongY"] = this.canTranslateAlongY;
        data["canTranslateAlongZ"] = this.canTranslateAlongZ;
        data["canRotateAboutX"] = this.canRotateAboutX;
        data["canRotateAboutY"] = this.canRotateAboutY;
        data["canRotateAboutZ"] = this.canRotateAboutZ;
        return data;
    }
}

export interface IRestraint {
    canTranslateAlongX: boolean;
    canTranslateAlongY: boolean;
    canTranslateAlongZ: boolean;
    canRotateAboutX: boolean;
    canRotateAboutY: boolean;
    canRotateAboutZ: boolean;

    [key: string]: any;
}

export class ResultOfModelResponse2 implements IResultOfModelResponse2 {
    value!: ModelResponse | undefined;
    error!: Exception | undefined;
    isError!: boolean;

    [key: string]: any;

    constructor(data?: IResultOfModelResponse2) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.value = _data["value"] ? ModelResponse.fromJS(_data["value"]) : <any>undefined;
            this.error = _data["error"] ? Exception.fromJS(_data["error"]) : <any>undefined;
            this.isError = _data["isError"];
        }
    }

    static fromJS(data: any): ResultOfModelResponse2 {
        data = typeof data === 'object' ? data : {};
        let result = new ResultOfModelResponse2();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["value"] = this.value ? this.value.toJSON() : <any>undefined;
        data["error"] = this.error ? this.error.toJSON() : <any>undefined;
        data["isError"] = this.isError;
        return data;
    }
}

export interface IResultOfModelResponse2 {
    value: ModelResponse | undefined;
    error: Exception | undefined;
    isError: boolean;

    [key: string]: any;
}

export class ResultOfNodeResponse implements IResultOfNodeResponse {
    value!: NodeResponse | undefined;
    error!: Exception | undefined;
    isError!: boolean;

    [key: string]: any;

    constructor(data?: IResultOfNodeResponse) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.value = _data["value"] ? NodeResponse.fromJS(_data["value"]) : <any>undefined;
            this.error = _data["error"] ? Exception.fromJS(_data["error"]) : <any>undefined;
            this.isError = _data["isError"];
        }
    }

    static fromJS(data: any): ResultOfNodeResponse {
        data = typeof data === 'object' ? data : {};
        let result = new ResultOfNodeResponse();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["value"] = this.value ? this.value.toJSON() : <any>undefined;
        data["error"] = this.error ? this.error.toJSON() : <any>undefined;
        data["isError"] = this.isError;
        return data;
    }
}

export interface IResultOfNodeResponse {
    value: NodeResponse | undefined;
    error: Exception | undefined;
    isError: boolean;

    [key: string]: any;
}

export class UnitSettingsContract implements IUnitSettingsContract {
    lengthUnit!: number;
    forceUnit!: number;
    angleUnit?: number;

    [key: string]: any;

    constructor(data?: IUnitSettingsContract) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.lengthUnit = _data["lengthUnit"];
            this.forceUnit = _data["forceUnit"];
            this.angleUnit = _data["angleUnit"];
        }
    }

    static fromJS(data: any): UnitSettingsContract {
        data = typeof data === 'object' ? data : {};
        let result = new UnitSettingsContract();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["lengthUnit"] = this.lengthUnit;
        data["forceUnit"] = this.forceUnit;
        data["angleUnit"] = this.angleUnit;
        return data;
    }
}

export interface IUnitSettingsContract {
    lengthUnit: number;
    forceUnit: number;
    angleUnit?: number;

    [key: string]: any;
}

export class UpdateNodeRequest implements IUpdateNodeRequest {
    id!: number;
    locationPoint!: NullableOfPartialPoint | undefined;
    restraint!: NullableOfPartialRestraint | undefined;

    [key: string]: any;

    constructor(data?: IUpdateNodeRequest) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            for (var property in _data) {
                if (_data.hasOwnProperty(property))
                    this[property] = _data[property];
            }
            this.id = _data["id"];
            this.locationPoint = _data["locationPoint"] ? NullableOfPartialPoint.fromJS(_data["locationPoint"]) : <any>undefined;
            this.restraint = _data["restraint"] ? NullableOfPartialRestraint.fromJS(_data["restraint"]) : <any>undefined;
        }
    }

    static fromJS(data: any): UpdateNodeRequest {
        data = typeof data === 'object' ? data : {};
        let result = new UpdateNodeRequest();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        for (var property in this) {
            if (this.hasOwnProperty(property))
                data[property] = this[property];
        }
        data["id"] = this.id;
        data["locationPoint"] = this.locationPoint ? this.locationPoint.toJSON() : <any>undefined;
        data["restraint"] = this.restraint ? this.restraint.toJSON() : <any>undefined;
        return data;
    }
}

export interface IUpdateNodeRequest {
    id: number;
    locationPoint: NullableOfPartialPoint | undefined;
    restraint: NullableOfPartialRestraint | undefined;

    [key: string]: any;
}

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new ApiException(message, status, response, headers, null);
}