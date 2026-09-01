import { useStorage, type RemovableRef } from "@vueuse/core";
import { watch } from "vue";

export interface StorageBackedRefHandle<T> {
  get: () => T;
  set: (value: T) => void;
}

export function useStorageBackedRef<T>(
  key: string,
  handle: StorageBackedRefHandle<T>,
): RemovableRef<T> {
  const storedValue = useStorage<T>(key, handle.get());

  watch(storedValue, handle.set, { immediate: true });
  watch(handle.get, (value) => {
    storedValue.value = value;
  });

  return storedValue;
}
