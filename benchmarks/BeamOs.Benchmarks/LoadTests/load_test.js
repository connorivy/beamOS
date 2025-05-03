import http from 'k6/http';
// import { StructuralAnalysisApiClientV1 } from '../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1.js';
import { ResultOfAnalyticalResultsResponse } from '../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1.js';

import { sleep, check } from 'k6';


export let options = {
    insecuteSkipTLSVerify: true,
    noConnectionReuse: true,
    // stages: [
    //     { duration: '5m', target: 100 },
    //     { duration: '10m', target: 100 },
    //     { duration: '5m', target: 0 },
    // ],
    vus: 1,
    duration: '1m',
};

export default function () {
    const url = 'http://localhost:5223';
    const modelId = '4ce66084-4ac1-40bc-99ae-3d0f334c66fa';
    const x = new StructuralAnalysisApiClientV1(url);
    const loadComboResponse = x.createLoadCombination(modelId, { "LoadCaseFactors": { 1: 1.0 } });
    const loadComboId = loadComboResponse.body.value.id;
    const res = x.runOpenSeesAnalysis("4ce66084-4ac1-40bc-99ae-3d0f334c66fa", { "UnitsOverride": null, "LoadCominationIds": [loadComboId] });
    check(res, {
        'status was 200': (r) => r.status === 200,
        // 'response time was < 200ms': (r) => r.timings.duration < 200,
    });

    sleep(10);
}

export class StructuralAnalysisApiClientV1 {

    constructor(baseUrl) {
        this.baseUrl = baseUrl !== null && baseUrl !== void 0 ? baseUrl : "http://localhost:5079";
    }

    createLoadCombination(modelId, body) {
        let url_ = this.baseUrl + "/api/models/{modelId}/load-combinations";
        if (modelId === undefined || modelId === null)
            throw new Error("The parameter 'modelId' must be defined.");
        url_ = url_.replace("{modelId}", encodeURIComponent("" + modelId));
        url_ = url_.replace(/[?&]$/, "");
        const content_ = JSON.stringify(body);
        let options_ = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };
        // return this.http.fetch(url_, options_).then((_response) => {
        //     return this.processCreateLoadCombination(_response);
        // });
        const response = http.post(url_, content_, options_);
        console.log("Response: ", response);
        return response;
    }

    runOpenSeesAnalysis(modelId, body) {
        let url_ = this.baseUrl + "/api/models/{modelId}/analyze/opensees";
        if (modelId === undefined || modelId === null)
            throw new Error("The parameter 'modelId' must be defined.");
        url_ = url_.replace("{modelId}", encodeURIComponent("" + modelId));
        url_ = url_.replace(/[?&]$/, "");
        const content_ = JSON.stringify(body);
        let options_ = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };
        // return this.http.fetch(url_, options_).then((_response) => {
        //     console.log("Response: ", _response);
        //     return this.processRunOpenSeesAnalysis(_response);
        // });
        const response = http.post(url_, content_, options_);
        console.log("Response: ", response);
        return response;
    }
    processRunOpenSeesAnalysis(response) {
        const status = response.status;
        let _headers = {};
        if (response.headers && response.headers.forEach) {
            response.headers.forEach((v, k) => _headers[k] = v);
        }
        ;
        if (status === 200) {
            return response.text().then((_responseText) => {
                let result200 = null;
                let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
                result200 = ResultOfAnalyticalResultsResponse.fromJS(resultData200);
                return result200;
            });
        }
        else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
                return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve(null);
    }
}