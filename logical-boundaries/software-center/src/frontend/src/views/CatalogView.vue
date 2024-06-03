<script setup lang="ts">
import Button from 'primevue/button'
import Column from 'primevue/column'
import DataTable from 'primevue/datatable'

import CreateSoftwareItem from '@/components/CreateSofwareItem.vue'
import type { CatalogItem } from '@/composables/types'
import useCatalogRetire from '@/composables/useCatalogMutation'
import { useCatalogQuery } from '@/composables/useCatalogQuery'
import Card from 'primevue/card'
import ConfirmPopup from 'primevue/confirmpopup'
import Toast from 'primevue/toast'
import { useConfirm } from 'primevue/useconfirm'
import { ref } from 'vue'
const confirm = useConfirm()

const { mutate: retireItem } = useCatalogRetire()
const confirmRetire = (event: { currentTarget: any }, item: CatalogItem) => {
  confirm.require({
    target: event.currentTarget,
    message: `Are you sure you want to retire ${item.title}?`,
    accept: () => {
      retireItem(item)
    }
  })
}

const { isPending, isError, data, error } = useCatalogQuery()

const selectedItem = ref()
</script>
<template>
  <Toast />
  <ConfirmPopup></ConfirmPopup>

  <Card>
    <template #header>
      <h2 class="text-purple-600">Software Catalog</h2>
    </template>
    <template #content>
      <p v-if="isPending">Loading</p>
      <p v-else-if="isError">Error {{ error?.status }}</p>
      <div v-else>
        <CreateSoftwareItem />
        <DataTable
          scrollable
          scrollHeight="600px"
          removableSort
          :value="data"
          size="large"
          sortField="title"
          :sort-order="1"
          selection-mode="single"
          v-model:selection="selectedItem"
        >
          <template #empty>
            <div class="p-4 text-center">No items in catalog</div>
          </template>
          <Column field="title" sortable header="Title" />
          <Column field="description" sortable header="Description" />
          <Column header="Opts">
            <template #body="slotProps">
              <RouterLink :to="{ name: 'catalog-issues', params: { id: slotProps.data.id } }">
                <Button type="button" label="Issues" badge="2" severity="success" class="mr-2" />
              </RouterLink>
              <Button
                label="Retire"
                severity="warning"
                @click="confirmRetire($event, slotProps.data)"
              />
            </template>
          </Column>
        </DataTable>
      </div>
    </template>
  </Card>
</template>
