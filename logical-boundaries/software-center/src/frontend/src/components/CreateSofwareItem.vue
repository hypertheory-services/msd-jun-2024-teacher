<script setup lang="ts">
import type { CatalogItem } from '@/composables/types'
import { useMutation, useQueryClient } from '@tanstack/vue-query'
import axios from 'axios'
import Button from 'primevue/button'
import Card from 'primevue/card'
import FloatLabel from 'primevue/floatlabel'
import InputText from 'primevue/inputtext'
import { ref } from 'vue'
const queryClient = useQueryClient()
const itemTitle = ref('')
const itemDescription = ref('')
const { isPending, mutate } = useMutation({
  mutationFn: ({ title, description }: { title: string; description: string }) =>
    axios
      .post<{
        id: string
        title: string
        description: string
      }>('/api/catalog', { title, description })
      .then((r) => r.data),
  onMutate: async (variables) => {
    await queryClient.cancelQueries({ queryKey: ['catalog'] })

    const tempItem = {
      id: crypto.randomUUID(),
      title: variables.title,
      description: variables.description
    }

    queryClient.setQueryData(['catalog'], (old: CatalogItem[]) => [...old, tempItem])

    return { tempItem: tempItem }
  },

  onSuccess: (result, _, context) => {
    queryClient.setQueryData(['catalog'], (old: CatalogItem[]) =>
      old.map((item) => (item.id === context.tempItem.id ? result : item))
    )
    itemTitle.value = ''
    itemDescription.value = ''
  }
})

function addItem() {
  mutate({ title: itemTitle.value, description: itemDescription.value })
}
</script>

<template>
  <div v-if="isPending">...</div>
  <Card v-else>
    <template #content>
      <form @submit.prevent="addItem" class="flex gap-2">
        <FloatLabel>
          <InputText id="title" v-model="itemTitle" required />
          <label for="title">Title</label>
        </FloatLabel>
        <FloatLabel>
          <InputText id="description" v-model="itemDescription" required />
          <label for="Description">Description</label>
        </FloatLabel>
        <Button label="Add" type="submit" severity="success" />
      </form>
    </template>
  </Card>
</template>
