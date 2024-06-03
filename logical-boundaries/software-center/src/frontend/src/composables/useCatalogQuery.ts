import { useQuery, type UseQueryReturnType } from '@tanstack/vue-query'
import axios, { AxiosError } from 'axios'
import type { CatalogItem, CatalogItemIssue } from './types'

const useCatalogQuery = (): UseQueryReturnType<CatalogItem[], AxiosError> => {
  return useQuery({
    queryKey: ['catalog'],
    queryFn: getCatalog
  })
}

const useCatalogItemQuery = (id: string): UseQueryReturnType<CatalogItem, AxiosError> => {
  return useQuery({
    queryKey: ['catalog', id],
    queryFn: () => getCatalogItem(id)
  })
}

const useCatalogItemIssuesQuery = (
  id: string
): UseQueryReturnType<CatalogItemIssue[], AxiosError> => {
  return useQuery({
    queryKey: ['catalog', id, 'issues'],
    queryFn: () => getCatalogItemIssues(id)
  })
}
function getCatalog() {
  return axios.get<CatalogItem[]>('/api/catalog').then((r) => r.data)
}

function getCatalogItem(id: string) {
  return axios.get<CatalogItem>(`/api/catalog/${id}`).then((r) => r.data)
}

function getCatalogItemIssues(id: string) {
  return axios.get<CatalogItemIssue[]>(`/api/catalog/${id}/issues`).then((r) => r.data)
}
export { useCatalogQuery, useCatalogItemQuery, useCatalogItemIssuesQuery }
