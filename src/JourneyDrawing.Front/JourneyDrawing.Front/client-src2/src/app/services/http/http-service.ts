import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Scenario } from 'src/app/model/scenario';

export type Endpoint =
  | 'Calculation/CalculateAllocation'
  | 'Calculation/CalculateSimulation'
  | 'Calculation/CanRunCalculation'
  | 'Files/UploadTransportationDetails'
  | 'Files/UploadShippingData'
  | 'Files/GetScenarioFiles'
  | 'Files/UploadPlanning'
  | 'Files/UploadContainers'
  | 'Files/UploadPorts'
  | 'Files/UploadCategories'
  | 'Files/UploadParameters'
  | 'Files/UploadVesselTypes'
  | 'Files/UploadTransportDistances'
  | 'Files/UploadTransportDurations'
  | 'Files/UploadCharters'
  | 'Scenario/GetScenarios'
  | 'Scenario/GetScenario'
  | 'Scenario/DeleteScenario'
  | 'Scenario/UpdateScenario'
  | 'Scenario/CreateScenario';

export type DocumentFile =
  | 'TransportationDetails'
  | 'ShippingData'
  | 'Containers'
  | 'Planning';

@Injectable({
  providedIn: 'root',
})
export class HttpService {
  private readonly BaseUrl: string = environment.backendURL;
  constructor(private httpClient: HttpClient) {}

  public get = <TOutput>(
    relativeUrl: Endpoint,
    search?: string,
    addApiV1: boolean = true
  ) => {
    let url = !search
      ? `${this.BaseUrl}/api/v1/${relativeUrl}`
      : `${this.BaseUrl}/api/v1/${relativeUrl}?${search ?? ''}`;

    if (!addApiV1) {
      url = !search
        ? `${this.BaseUrl}/${relativeUrl}`
        : `${this.BaseUrl}/${relativeUrl}?${search ?? ''}`;
    }

    let resultObservable = this.httpClient.get<TOutput>(url);
    return resultObservable;
  };

  public post = <TInput, TOutput>(relativeUrl: Endpoint, body: TInput) => {
    let resultObservable = this.httpClient.post<TOutput>(
      `${this.BaseUrl}/api/v1/${relativeUrl}`,
      body
    );
    return resultObservable;
  };

  public postFile = <TInput, TOutput>(relativeUrl: Endpoint, body: TInput) => {
    var formData = new FormData();
    for (let paramName in body) {
      let bodyAsAny: any = body;
      formData.set(paramName, bodyAsAny[paramName]);
    }
    let resultObservable = this.httpClient.post<TOutput>(
      `${this.BaseUrl}/api/v1/${relativeUrl}`,
      formData
    );
    return resultObservable;
  };

  public delete = <TOutput>(relativeUrl: Endpoint, search?: string) => {
    let url = !search
      ? `${this.BaseUrl}/api/v1/${relativeUrl}`
      : `${this.BaseUrl}/api/v1/${relativeUrl}?${search ?? ''}`;
    return this.httpClient.delete<TOutput>(url);
  };

  public put = <TInput, TOutput>(relativeUrl: Endpoint, body: TInput) => {
    let resultObservable = this.httpClient.put<TOutput>(
      `${this.BaseUrl}/api/v1/${relativeUrl}`,
      body
    );
    return resultObservable;
  };

  public getBlob(file: DocumentFile, scenario: Scenario) {
    let endpoint = '';
    switch (file) {
      case 'TransportationDetails': {
        endpoint = 'DownloadTransportationDetails';
        break;
      }
      case 'ShippingData': {
        endpoint = 'DownloadShippingData';
        break;
      }
      case 'Planning': {
        endpoint = 'DownloadPlanning';
        break;
      }
      case 'Containers': {
        endpoint = 'DownloadContainers';
        break;
      }
      default: {
        throw Error('unknown file type to get');
      }
    }
    return this.httpClient.get(
      `${this.BaseUrl}/api/v1/Files/${endpoint}?PartitionKey=${scenario.partitionKey}&RowKey=${scenario.rowKey}`,
      {
        observe: 'response',
        responseType: 'arraybuffer',
      }
    );
  }
}
