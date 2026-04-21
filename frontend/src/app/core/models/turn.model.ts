export interface Turn {
  turnId: number;
  turnNumber: string;
  clientId: number;
  clientName: string;
  serviceId: number;
  serviceName: string;
  status: string;
  priority: number;
  createdAt: string;
  attendedAt?: string;
}

export interface CreateTurnRequest {
  clientId: number;
  serviceId: number;
  priority?: number;
}

export interface UpdateTurnRequest {
  status: string;
  priority?: number;
}
