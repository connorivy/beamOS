//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.0.0.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

export interface IIdentityAlphaClient {

    registerEndpoint(request: LoginRequest): Promise<boolean>;

    refreshEndpoint(token: string): Promise<string>;

    loginEndpoint(request: LoginRequest): Promise<AuthenticationResponse>;

    loginWithGoogleEndpointRedirect(redirectUrl: string): Promise<string>;

    /**
     * @return Success
     */
    beamOsIdentityApiFeaturesLoginWithGoogleLoginWithGoogleEndpoint(externalLoginRequest: ExternalLoginRequest): Promise<void>;
}

export class IdentityAlphaClient implements IIdentityAlphaClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl ?? "";
    }

    registerEndpoint(request: LoginRequest): Promise<boolean> {
        let url_ = this.baseUrl + "/register";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(request);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processRegisterEndpoint(_response);
        });
    }

    protected processRegisterEndpoint(response: Response): Promise<boolean> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result200 = resultData200 !== undefined ? resultData200 : <any>null;
    
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<boolean>(null as any);
    }

    refreshEndpoint(token: string): Promise<string> {
        let url_ = this.baseUrl + "/refresh?";
        if (token === undefined || token === null)
            throw new Error("The parameter 'token' must be defined and cannot be null.");
        else
            url_ += "token=" + encodeURIComponent("" + token) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "POST",
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processRefreshEndpoint(_response);
        });
    }

    protected processRefreshEndpoint(response: Response): Promise<string> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result200 = resultData200 !== undefined ? resultData200 : <any>null;
    
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<string>(null as any);
    }

    loginEndpoint(request: LoginRequest): Promise<AuthenticationResponse> {
        let url_ = this.baseUrl + "/login";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(request);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processLoginEndpoint(_response);
        });
    }

    protected processLoginEndpoint(response: Response): Promise<AuthenticationResponse> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = AuthenticationResponse.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<AuthenticationResponse>(null as any);
    }

    loginWithGoogleEndpointRedirect(redirectUrl: string): Promise<string> {
        let url_ = this.baseUrl + "/login-with-google/authenticated?";
        if (redirectUrl === undefined || redirectUrl === null)
            throw new Error("The parameter 'redirectUrl' must be defined and cannot be null.");
        else
            url_ += "redirectUrl=" + encodeURIComponent("" + redirectUrl) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "text/plain"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processLoginWithGoogleEndpointRedirect(_response);
        });
    }

    protected processLoginWithGoogleEndpointRedirect(response: Response): Promise<string> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result200 = resultData200 !== undefined ? resultData200 : <any>null;
    
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<string>(null as any);
    }

    /**
     * @return Success
     */
    beamOsIdentityApiFeaturesLoginWithGoogleLoginWithGoogleEndpoint(externalLoginRequest: ExternalLoginRequest): Promise<void> {
        let url_ = this.baseUrl + "/login-with-google";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = Object.keys(externalLoginRequest as any).map((key) => {
            return encodeURIComponent(key) + '=' + encodeURIComponent((externalLoginRequest as any)[key]);
        }).join('&')

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/x-www-form-urlencoded",
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processBeamOsIdentityApiFeaturesLoginWithGoogleLoginWithGoogleEndpoint(_response);
        });
    }

    protected processBeamOsIdentityApiFeaturesLoginWithGoogleLoginWithGoogleEndpoint(response: Response): Promise<void> {
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            return;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<void>(null as any);
    }
}

export class LoginRequest implements ILoginRequest {
    email!: string;
    password!: string;

    constructor(data?: ILoginRequest) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.email = _data["email"];
            this.password = _data["password"];
        }
    }

    static fromJS(data: any): LoginRequest {
        data = typeof data === 'object' ? data : {};
        let result = new LoginRequest();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["email"] = this.email;
        data["password"] = this.password;
        return data;
    }
}

export interface ILoginRequest {
    email: string;
    password: string;
}

export class AuthenticationResponse implements IAuthenticationResponse {
    accessToken!: string;
    refreshToken!: string;

    constructor(data?: IAuthenticationResponse) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.accessToken = _data["accessToken"];
            this.refreshToken = _data["refreshToken"];
        }
    }

    static fromJS(data: any): AuthenticationResponse {
        data = typeof data === 'object' ? data : {};
        let result = new AuthenticationResponse();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["accessToken"] = this.accessToken;
        data["refreshToken"] = this.refreshToken;
        return data;
    }
}

export interface IAuthenticationResponse {
    accessToken: string;
    refreshToken: string;
}

export class ExternalLoginRequest implements IExternalLoginRequest {
    returnUrl!: string;
    provider!: string;
    __RequestVerificationToken!: string;

    constructor(data?: IExternalLoginRequest) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.returnUrl = _data["returnUrl"];
            this.provider = _data["provider"];
            this.__RequestVerificationToken = _data["__RequestVerificationToken"];
        }
    }

    static fromJS(data: any): ExternalLoginRequest {
        data = typeof data === 'object' ? data : {};
        let result = new ExternalLoginRequest();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["returnUrl"] = this.returnUrl;
        data["provider"] = this.provider;
        data["__RequestVerificationToken"] = this.__RequestVerificationToken;
        return data;
    }
}

export interface IExternalLoginRequest {
    returnUrl: string;
    provider: string;
    __RequestVerificationToken: string;
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