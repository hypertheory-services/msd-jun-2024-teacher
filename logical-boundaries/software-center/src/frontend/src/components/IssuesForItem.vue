<script setup lang="ts">
import { useCatalogItemIssuesQuery, useCatalogItemQuery } from '@/composables/useCatalogQuery'
import Button from 'primevue/button'
const props = defineProps({
  id: String
})

const { data, isError, isPending } = useCatalogItemQuery(props.id || '')

const {
  data: issues,
  isPending: issuesPending,
  isError: issuesError
} = useCatalogItemIssuesQuery(props.id || '')
</script>

<template>
  <div v-if="isPending">Loading</div>
  <div v-else-if="isError">No Catalog Item with That Id</div>
  <div v-else-if="data">
    <h1>{{ data.title }}</h1>
    <p>{{ data.description }}</p>
    <div v-if="issuesPending">Loading Issues</div>
    <div v-else-if="issuesError">Error Loading Issues</div>
    <div v-else-if="issues">
      <h2>Issues</h2>
      <pre>{{ issues }}</pre>
    </div>
  </div>

  <div>
    <RouterLink to="/catalog">
      <Button label="Back to Catalog" severity="success" />
    </RouterLink>
  </div>
</template>
