export interface Client {
  clientId: number;
  identification: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
  createdAt: string;
  status: string;
}

export interface CreateClientRequest {
  identification: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
}

export interface UpdateClientRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address: string;
}
