<?php

    echo "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
    echo "<vxml version=\"2.0\">";

    $con = mysqli_connect('127.0.0.1', 'root', '', "bank");
    $kontonr = $_GET['kontonr'];
    $pin = $_GET['pin'];
    $mode = $_GET['mode']; // raise || transfer

    if ($mode === "raise") {
        $query_guthaben = "UPDATE kontostand SET guthaben=guthaben + 1000 WHERE kontonummer=$kontonr";
        $query_credit = "UPDATE kunde SET Kreditanzahl=Kreditanzahl + 1 WHERE kontonummer=$kontonr";
        mysqli_query($con, $query_guthaben);
        mysqli_query($con, $query_credit);
    }

    if ($mode === "transfer") {
        $targetNr = $_GET['targetNr'];
        $amount = $_GET['amount'];
        $query_owner = "UPDATE kontostand SET guthaben=guthaben - $amount WHERE kontonummer=$kontonr";
        $query_target = "UPDATE kontostand SET guthaben=guthaben + $amount WHERE kontonummer=$targetNr";
        mysqli_query($con, $query_owner);
        mysqli_query($con, $query_target);
    }

    

?>

    <form id="raiseCredit">
        <block>
            <prompt>You still have credit left. Your account balance has been raised by 1000.</prompt>
            <goto next="login.php?kontonr=<?php echo $kontonr; ?>&amp;<?php echo $pin; ?>#accountHandlingMenu" />
        </block>
    </form>

    <form id="transfer">
        <block>
			<prompt>Your transfer was successful. You will be redirected to the menu.</prompt>
			<goto next="login.php?kontonr=<?php echo $kontonr; ?>&amp;<?php echo $pin; ?>#accountHandlingMenu" />
		</block>
    </form>


</vxml>