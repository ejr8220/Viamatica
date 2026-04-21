export interface Service {
  serviceId: number;
  name: string;
  code: string;
  description: string;
  estimatedTimeMinutes: number;
  isActive: boolean;
  createdAt: string;
}

export interface CreateServiceRequest {
  name: string;
  code: string;
  description: string;
  estimatedTimeMinutes: number;
}

export interface UpdateServiceRequest {
  name: string;
  description: string;
  estimatedTimeMinutes: number;
  isActive: boolean;
}
