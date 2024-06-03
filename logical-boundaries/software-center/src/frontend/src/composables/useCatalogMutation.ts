import { useQueryClient, useMutation, type UseMutationReturnType } from '@tanstack/vue-query'
import type { CatalogItem } from './types'
import axios, { AxiosError } from 'axios'

import { useToast } from 'primevue/usetoast'

export default function useCatalogRetireItem(): UseMutationReturnType<
  undefined,
  AxiosError<unknown, any>,
  CatalogItem,
  unknown
> {
  const queryClient = useQueryClient()
  const toast = useToast()
  return useMutation({
    mutationFn: (item: CatalogItem) =>
      axios.delete(`/api/catalog/${item.id}`).then((_) => undefined),
    onSuccess: (_, item) => {
      queryClient.invalidateQueries({
        queryKey: ['catalog']
      })
      toast.add({
        severity: 'success',
        summary: 'Item Retired',
        detail: `Item ${item.title} has been retired`,
        life: 3000
      })
    }
  })
}
