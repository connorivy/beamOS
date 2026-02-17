
# ApiModelsModelIdBranchesBranchNameRevisionGet200ResponseModelRevision


## Properties

Name | Type
------------ | -------------
`id` | string
`modelId` | string
`name` | string
`parentRevisionId` | string
`secondParentRevisionId` | string
`authorId` | string
`message` | string
`createdAt` | Date
`nodes` | [Array&lt;ApiModelsModelIdBranchesBranchNameNodesBatchPost200ResponseNodesInner&gt;](ApiModelsModelIdBranchesBranchNameNodesBatchPost200ResponseNodesInner.md)
`materials` | [Array&lt;ApiModelsModelIdBranchesBranchNameMaterialsBatchPut200ResponseMaterialsInner&gt;](ApiModelsModelIdBranchesBranchNameMaterialsBatchPut200ResponseMaterialsInner.md)
`modelSettings` | [ApiModelsModelIdBranchesBranchNameRevisionGet200ResponseModelRevisionModelSettings](ApiModelsModelIdBranchesBranchNameRevisionGet200ResponseModelRevisionModelSettings.md)
`sectionProfiles` | [Array&lt;ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPost200ResponseSectionProfilesInner&gt;](ApiModelsModelIdBranchesBranchNameSectionProfilesBatchPost200ResponseSectionProfilesInner.md)
`element1ds` | [Array&lt;ApiModelsModelIdBranchesBranchNameElement1dsBatchPost200ResponseElement1dsInner&gt;](ApiModelsModelIdBranchesBranchNameElement1dsBatchPost200ResponseElement1dsInner.md)

## Example

```typescript
import type { ApiModelsModelIdBranchesBranchNameRevisionGet200ResponseModelRevision } from ''

// TODO: Update the object below with actual values
const example = {
  "id": null,
  "modelId": null,
  "name": null,
  "parentRevisionId": null,
  "secondParentRevisionId": null,
  "authorId": null,
  "message": null,
  "createdAt": null,
  "nodes": null,
  "materials": null,
  "modelSettings": null,
  "sectionProfiles": null,
  "element1ds": null,
} satisfies ApiModelsModelIdBranchesBranchNameRevisionGet200ResponseModelRevision

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as ApiModelsModelIdBranchesBranchNameRevisionGet200ResponseModelRevision
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


