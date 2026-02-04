import api from '@/services/api'

export interface RepertoireItem {
  id: string
  parentId?: string | null
  name: string
  type: number
  color: number
  sortOrder: number
}

export async function getRepertoireTree(): Promise<RepertoireItem[]> {
  const { data } = await api.get('/repertoire')
  return data
}

export async function createOpening(payload: {
  parentId?: string | null
  name: string
  color: number
}) {
  const { data } = await api.post('/repertoire/opening', payload)
  return data as string // openingId
}
