import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { VehicleSearchResultDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection?: signalR.HubConnection;
  public vehicleSearchResults = signal<VehicleSearchResultDto | null>(null);
  public connectionState = signal<'connected' | 'disconnected' | 'connecting'>('disconnected');

  constructor() {}

  startConnection(): void {
    if (this.hubConnection) {
      return;
    }

    this.connectionState.set('connecting');
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/vehicle-search') // TODO: Move to environment config
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
        this.connectionState.set('connected');
        this.registerHandlers();
      })
      .catch(err => {
        console.error('Error while starting SignalR connection: ', err);
        this.connectionState.set('disconnected');
      });

    this.hubConnection.onreconnecting(() => {
      this.connectionState.set('connecting');
    });

    this.hubConnection.onreconnected(() => {
      this.connectionState.set('connected');
    });

    this.hubConnection.onclose(() => {
      this.connectionState.set('disconnected');
    });
  }

  private registerHandlers(): void {
    if (!this.hubConnection) {
      return;
    }

    this.hubConnection.on('VehicleSearchUpdate', (data: VehicleSearchResultDto) => {
      console.log('Received vehicle search update:', data);
      this.vehicleSearchResults.set(data);
    });
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = undefined;
      this.connectionState.set('disconnected');
    }
  }
}
