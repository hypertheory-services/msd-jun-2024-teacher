<script setup lang="ts">
import TabMenu from 'primevue/tabmenu'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'

const { currentRoute } = useRouter()

const items = ref([
  { label: 'Home', icon: 'pi pi-home', route: '/' },
  { label: 'Catalog', icon: 'pi pi-list', route: '/catalog' }
])

const isCurrentRoute = computed(() => {
  return items.value.findIndex((i) => i.route == currentRoute.value.path)
})
</script>
<template>
  <div class="card">
    <TabMenu :model="items" :activeIndex="isCurrentRoute">
      <template #item="{ item, props }">
        <router-link v-slot="{ href, navigate }" :to="item.route" custom>
          <a v-ripple :href="href" v-bind="props.action" @click="navigate">
            <span v-bind="props.icon" />
            <span v-bind="props.label">{{ item.label }}</span>
          </a>
        </router-link>
      </template>
    </TabMenu>
  </div>
</template>
