export interface Device {
  id: number;
  name: string;
  manufacturer: string;
  type: string;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ram: number;
  description?: string;
  isAvailable: boolean;
  currentUserId?: number;
  currentUserName?: string;
  currentLocation?: string;
}

export interface DeviceRequest {
  name: string;
  manufacturer: string;
  type: number;
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ram: number;
  description?: string;
}
