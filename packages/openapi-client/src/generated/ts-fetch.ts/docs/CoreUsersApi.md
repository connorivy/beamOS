# CoreUsersApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**apiElement1dsElement1dIdGet**](CoreUsersApi.md#apielement1dselement1didget) | **GET** /api/element1ds/{element1dId} |  |
| [**apiMaterialsMaterialIdGet**](CoreUsersApi.md#apimaterialsmaterialidget) | **GET** /api/materials/{materialId} |  |
| [**apiModelsGet**](CoreUsersApi.md#apimodelsget) | **GET** /api/models |  |
| [**apiModelsModelIdBranchesBranchNameElement1dsBatchPost**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnameelement1dsbatchpostoperation) | **POST** /api/models/{modelId}/branches/{branchName}/element1ds/batch |  |
| [**apiModelsModelIdBranchesBranchNameMaterialsBatchPost**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamematerialsbatchpostoperation) | **POST** /api/models/{modelId}/branches/{branchName}/materials/batch |  |
| [**apiModelsModelIdBranchesBranchNameMaterialsBatchPut**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamematerialsbatchputoperation) | **PUT** /api/models/{modelId}/branches/{branchName}/materials/batch |  |
| [**apiModelsModelIdBranchesBranchNameModelSettingsPut**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamemodelsettingsputoperation) | **PUT** /api/models/{modelId}/branches/{branchName}/model-settings |  |
| [**apiModelsModelIdBranchesBranchNameNodesBatchPost**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamenodesbatchpostoperation) | **POST** /api/models/{modelId}/branches/{branchName}/nodes/batch |  |
| [**apiModelsModelIdBranchesBranchNameRevisionGet**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamerevisionget) | **GET** /api/models/{modelId}/branches/{branchName}/revision |  |
| [**apiModelsModelIdBranchesBranchNameRevisionsPost**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamerevisionspostoperation) | **POST** /api/models/{modelId}/branches/{branchName}/revisions |  |
| [**apiModelsModelIdBranchesBranchNameSectionProfilesBatchPost**](CoreUsersApi.md#apimodelsmodelidbranchesbranchnamesectionprofilesbatchpostoperation) | **POST** /api/models/{modelId}/branches/{branchName}/section-profiles/batch |  |
| [**apiModelsModelIdNodesNodeIdPatch**](CoreUsersApi.md#apimodelsmodelidnodesnodeidpatchoperation) | **PATCH** /api/models/{modelId}/nodes/{nodeId} |  |
| [**apiModelsModelIdPatch**](CoreUsersApi.md#apimodelsmodelidpatch) | **PATCH** /api/models/{modelId} |  |
| [**apiModelsPost**](CoreUsersApi.md#apimodelspostoperation) | **POST** /api/models |  |
| [**apiSectionProfilesSectionProfileIdGet**](CoreUsersApi.md#apisectionprofilessectionprofileidget) | **GET** /api/section-profiles/{sectionProfileId} |  |



## apiElement1dsElement1dIdGet

> ApiElement1dsElement1dIdGet200Response apiElement1dsElement1dIdGet(element1dId)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiElement1dsElement1dIdGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    element1dId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiElement1dsElement1dIdGetRequest;

  try {
    const data = await api.apiElement1dsElement1dIdGet(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **element1dId** | `string` |  | [Defaults to `undefined`] |

### Return type

[**ApiElement1dsElement1dIdGet200Response**](ApiElement1dsElement1dIdGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiMaterialsMaterialIdGet

> ApiMaterialsMaterialIdGet200Response apiMaterialsMaterialIdGet(materialId)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiMaterialsMaterialIdGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    materialId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiMaterialsMaterialIdGetRequest;

  try {
    const data = await api.apiMaterialsMaterialIdGet(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **materialId** | `string` |  | [Defaults to `undefined`] |

### Return type

[**ApiMaterialsMaterialIdGet200Response**](ApiMaterialsMaterialIdGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsGet

> ApiModelsGet200Response apiModelsGet()



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  try {
    const data = await api.apiModelsGet();
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters

This endpoint does not need any parameter.

### Return type

[**ApiModelsGet200Response**](ApiModelsGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameElement1dsBatchPost

> ApiModelsModelIdBranchesBranchNameElement1dsBatchPost200Response apiModelsModelIdBranchesBranchNameElement1dsBatchPost(modelId, branchName, apiModelsModelIdBranchesBranchNameElement1dsBatchPostRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameElement1dsBatchPostOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameElement1dsBatchPostRequest
    apiModelsModelIdBranchesBranchNameElement1dsBatchPostRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameElement1dsBatchPostOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameElement1dsBatchPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameElement1dsBatchPostRequest** | [ApiModelsModelIdBranchesBranchNameElement1dsBatchPostRequest](ApiModelsModelIdBranchesBranchNameElement1dsBatchPostRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameElement1dsBatchPost200Response**](ApiModelsModelIdBranchesBranchNameElement1dsBatchPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameMaterialsBatchPost

> ApiModelsModelIdBranchesBranchNameMaterialsBatchPost200Response apiModelsModelIdBranchesBranchNameMaterialsBatchPost(modelId, branchName, apiModelsModelIdBranchesBranchNameMaterialsBatchPostRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameMaterialsBatchPostOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameMaterialsBatchPostRequest
    apiModelsModelIdBranchesBranchNameMaterialsBatchPostRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameMaterialsBatchPostOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameMaterialsBatchPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameMaterialsBatchPostRequest** | [ApiModelsModelIdBranchesBranchNameMaterialsBatchPostRequest](ApiModelsModelIdBranchesBranchNameMaterialsBatchPostRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameMaterialsBatchPost200Response**](ApiModelsModelIdBranchesBranchNameMaterialsBatchPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameMaterialsBatchPut

> ApiModelsModelIdBranchesBranchNameMaterialsBatchPut200Response apiModelsModelIdBranchesBranchNameMaterialsBatchPut(modelId, branchName, apiModelsModelIdBranchesBranchNameMaterialsBatchPutRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameMaterialsBatchPutOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameMaterialsBatchPutRequest
    apiModelsModelIdBranchesBranchNameMaterialsBatchPutRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameMaterialsBatchPutOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameMaterialsBatchPut(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameMaterialsBatchPutRequest** | [ApiModelsModelIdBranchesBranchNameMaterialsBatchPutRequest](ApiModelsModelIdBranchesBranchNameMaterialsBatchPutRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameMaterialsBatchPut200Response**](ApiModelsModelIdBranchesBranchNameMaterialsBatchPut200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameModelSettingsPut

> ApiModelsModelIdBranchesBranchNameModelSettingsPut200Response apiModelsModelIdBranchesBranchNameModelSettingsPut(modelId, branchName, apiModelsModelIdBranchesBranchNameModelSettingsPutRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameModelSettingsPutOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameModelSettingsPutRequest
    apiModelsModelIdBranchesBranchNameModelSettingsPutRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameModelSettingsPutOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameModelSettingsPut(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameModelSettingsPutRequest** | [ApiModelsModelIdBranchesBranchNameModelSettingsPutRequest](ApiModelsModelIdBranchesBranchNameModelSettingsPutRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameModelSettingsPut200Response**](ApiModelsModelIdBranchesBranchNameModelSettingsPut200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameNodesBatchPost

> ApiModelsModelIdBranchesBranchNameNodesBatchPost200Response apiModelsModelIdBranchesBranchNameNodesBatchPost(modelId, branchName, apiModelsModelIdBranchesBranchNameNodesBatchPostRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameNodesBatchPostOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameNodesBatchPostRequest
    apiModelsModelIdBranchesBranchNameNodesBatchPostRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameNodesBatchPostOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameNodesBatchPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameNodesBatchPostRequest** | [ApiModelsModelIdBranchesBranchNameNodesBatchPostRequest](ApiModelsModelIdBranchesBranchNameNodesBatchPostRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameNodesBatchPost200Response**](ApiModelsModelIdBranchesBranchNameNodesBatchPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameRevisionGet

> ApiModelsModelIdBranchesBranchNameRevisionGet200Response apiModelsModelIdBranchesBranchNameRevisionGet(modelId, branchName)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameRevisionGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
  } satisfies ApiModelsModelIdBranchesBranchNameRevisionGetRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameRevisionGet(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |

### Return type

[**ApiModelsModelIdBranchesBranchNameRevisionGet200Response**](ApiModelsModelIdBranchesBranchNameRevisionGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameRevisionsPost

> ApiModelsModelIdBranchesBranchNameRevisionsPost200Response apiModelsModelIdBranchesBranchNameRevisionsPost(modelId, branchName, apiModelsModelIdBranchesBranchNameRevisionsPostRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameRevisionsPostOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameRevisionsPostRequest
    apiModelsModelIdBranchesBranchNameRevisionsPostRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameRevisionsPostOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameRevisionsPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameRevisionsPostRequest** | [ApiModelsModelIdBranchesBranchNameRevisionsPostRequest](ApiModelsModelIdBranchesBranchNameRevisionsPostRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameRevisionsPost200Response**](ApiModelsModelIdBranchesBranchNameRevisionsPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdBranchesBranchNameSectionProfilesBatchPost

> ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPost200Response apiModelsModelIdBranchesBranchNameSectionProfilesBatchPost(modelId, branchName, apiModelsModelIdBranchesBranchNameSectionProfilesBatchPostRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPostOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    branchName: branchName_example,
    // ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPostRequest
    apiModelsModelIdBranchesBranchNameSectionProfilesBatchPostRequest: ...,
  } satisfies ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPostOperationRequest;

  try {
    const data = await api.apiModelsModelIdBranchesBranchNameSectionProfilesBatchPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **branchName** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdBranchesBranchNameSectionProfilesBatchPostRequest** | [ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPostRequest](ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPostRequest.md) |  | |

### Return type

[**ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPost200Response**](ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdNodesNodeIdPatch

> ApiModelsModelIdNodesNodeIdPatch200Response apiModelsModelIdNodesNodeIdPatch(modelId, nodeId, apiModelsModelIdNodesNodeIdPatchRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdNodesNodeIdPatchOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // string
    nodeId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // ApiModelsModelIdNodesNodeIdPatchRequest
    apiModelsModelIdNodesNodeIdPatchRequest: ...,
  } satisfies ApiModelsModelIdNodesNodeIdPatchOperationRequest;

  try {
    const data = await api.apiModelsModelIdNodesNodeIdPatch(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **nodeId** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdNodesNodeIdPatchRequest** | [ApiModelsModelIdNodesNodeIdPatchRequest](ApiModelsModelIdNodesNodeIdPatchRequest.md) |  | |

### Return type

[**ApiModelsModelIdNodesNodeIdPatch200Response**](ApiModelsModelIdNodesNodeIdPatch200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsModelIdPatch

> ApiModelsModelIdPatch200Response apiModelsModelIdPatch(modelId, apiModelsModelIdNodesNodeIdPatchRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsModelIdPatchRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    modelId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
    // ApiModelsModelIdNodesNodeIdPatchRequest
    apiModelsModelIdNodesNodeIdPatchRequest: ...,
  } satisfies ApiModelsModelIdPatchRequest;

  try {
    const data = await api.apiModelsModelIdPatch(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **modelId** | `string` |  | [Defaults to `undefined`] |
| **apiModelsModelIdNodesNodeIdPatchRequest** | [ApiModelsModelIdNodesNodeIdPatchRequest](ApiModelsModelIdNodesNodeIdPatchRequest.md) |  | |

### Return type

[**ApiModelsModelIdPatch200Response**](ApiModelsModelIdPatch200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiModelsPost

> ApiModelsPost200Response apiModelsPost(apiModelsPostRequest)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiModelsPostOperationRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // ApiModelsPostRequest
    apiModelsPostRequest: ...,
  } satisfies ApiModelsPostOperationRequest;

  try {
    const data = await api.apiModelsPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **apiModelsPostRequest** | [ApiModelsPostRequest](ApiModelsPostRequest.md) |  | |

### Return type

[**ApiModelsPost200Response**](ApiModelsPost200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## apiSectionProfilesSectionProfileIdGet

> ApiSectionProfilesSectionProfileIdGet200Response apiSectionProfilesSectionProfileIdGet(sectionProfileId)



### Example

```ts
import {
  Configuration,
  CoreUsersApi,
} from '';
import type { ApiSectionProfilesSectionProfileIdGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new CoreUsersApi();

  const body = {
    // string
    sectionProfileId: 38400000-8cf0-11bd-b23e-10b96e4ef00d,
  } satisfies ApiSectionProfilesSectionProfileIdGetRequest;

  try {
    const data = await api.apiSectionProfilesSectionProfileIdGet(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **sectionProfileId** | `string` |  | [Defaults to `undefined`] |

### Return type

[**ApiSectionProfilesSectionProfileIdGet200Response**](ApiSectionProfilesSectionProfileIdGet200Response.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `application/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

