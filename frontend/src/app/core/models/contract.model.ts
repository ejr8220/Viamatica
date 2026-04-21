export interface Contract {
  contractId: number;
  contractNumber: string;
  clientId: number;
  clientName: string;
  serviceId: number;
  serviceName: string;
  startDate: string;
  endDate: string;
  amount: number;
  status: string;
  createdAt: string;
}

export interface CreateContractRequest {
  contractNumber: string;
  clientId: number;
  serviceId: number;
  startDate: string;
  endDate: string;
  amount: number;
}

export interface UpdateContractRequest {
  startDate: string;
  endDate: string;
  amount: number;
  status: string;
}
