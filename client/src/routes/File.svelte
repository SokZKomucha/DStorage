<script lang="ts">
  export let params: Record<string, string | undefined> = {};
  let fileDetailsPromise = getFileDetails();

  $: {
    console.log(params["id"])
    fileDetailsPromise = getFileDetails();
  }

  // Obviously not implemented yet, and won't be for a long time
  async function getFileDetails() {
    return new Promise<string>(r => {
      setTimeout(() => {
        r(params["id"]!)
      }, 350);
    });
  }
</script>

<div>
  {#await fileDetailsPromise}
    <p>Loading</p>
  {:then fileDetails} 
    <p>{fileDetails}</p>
  {/await}
</div>