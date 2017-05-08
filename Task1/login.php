<?php
    echo "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
	echo "<vxml version=\"2.0\">";

    $kontonr = $_GET['accountNumber'];
    $pin = $_GET['pin'];
    $succesful = "false";
	$account =  0;
	$credit = 0;

    $con = mysqli_connect('127.0.0.1', 'root', '', "bank");
    if (mysqli_connect_errno())
    {
    echo "Failed to connect to MySQL: " . mysqli_connect_error();
    }

    // mysqli_select_db($connection, "bank");
    $query_kunde = "SELECT * from kunde where Kontonummer=$kontonr and Pin=$pin";
    $result = mysqli_query($con, $query_kunde);
    $row = mysqli_fetch_array($result);
	
	$query_kontostand = "SELECT * FROM kontostand where Kontonummer=$kontonr";
	$result2 = mysqli_query($con, $query_kontostand);
	$row2 = mysqli_fetch_array($result2);

    if ($row != NULL) {
        $succesful = "true";
		$surname = $row[1];
		$name = $row[2];
		$credit = $row[4];
		$account = $row2[1];
    }

    
?>


<form>
	<block>
	<if cond="<?php echo $succesful; ?>">
		<prompt>You are now logged into your account, <?php echo $name ?> <?php echo $surname ?>.</prompt>
		<goto next="#accountHandlingMenu" />
	
	<else />
		<prompt>Your account number or pin were incorrect. The application will be closed.</prompt>
		<goto next="#goodbyeHandler" />
	</if>
	</block>
</form>

<menu id="accountHandlingMenu">
    <prompt>Choose from<enumerate/></prompt>
    <choice dtmf="1" next="#transferMoneyHandler">transfer money</choice>
    <choice dtmf="2" next="#callABankAgentHandler">call a bank agent</choice>
    <choice dtmf="3" next="#goodbyeHandler">goodbye</choice>
    <catch event="nomatch noinput help">
        <reprompt />
    </catch>
</menu>
	
	<form id="transferMoneyHandler">
		<block>
			<if cond="<?php echo $account ?> >= 0">
				<goto next="#transferMoney"/>
			<else/>
				<prompt>Your account balance is <say-as interpret-as="cardinal" format=","><?php echo $account ?></say-as>. You can not transfer money. Your credit will be checked.</prompt>
				<goto next="#checkCredit" />
			</if>
		</block>	
	</form>
	
	<form id="transferMoney">
		<field name="targetNr" type="number">
			<prompt>Please input the target account number.</prompt>
		</field>

		<field name="amount"  type="number">
			<prompt>Please input the the amount of money to transfer.</prompt>
		</field>	

		<block>
			<!--<if cond="amount < <?php echo $account; ?>">-->
				<goto next="http://127.0.0.1/bank/transfer.php?mode=transfer&amp;kontonr=<?php echo $kontonr ?>&amp;pin=<?php echo $pin ?>#transfer" namelist="targetNr amount"/>
			<!--<else/>-->
				<!--<prompt>Your account balance is not sufficient for this transfer. Consider raising your credit. Redirecting to Menu.</prompt>-->
				<!--<goto next="#accountHandlingMenu" />-->
			<!--</if>-->
		</block>
	</form>
	
	
	<form id="checkCredit">
		<block>
			<prompt>You have raised your credit <?php echo $credit; ?> times.</prompt>
			<if cond="<?php echo $credit; ?> ==2">
				<prompt>You have reached the maximum credit. You will be directed to a bank agent.</prompt>
				<goto next="#callABankAgentHandler" />
			<else/>
				<goto next="transfer.php?mode=raise&amp;kontonr=<?php echo $kontonr ?>&amp;<?php echo $pin ?>#raiseCredit"/>
			</if>
		</block>	
	</form>
	
	
	<form id="callABankAgentHandler">
		<block>
			<goto next="http://127.0.0.1/bank/welcome.vxml#callABankAgent"/>
		</block>
	</form>
	
	<form id="goodbyeHandler">
		<block>
			<goto next="http://127.0.0.1/bank/welcome.vxml#goodbye"/>
		</block>
	</form>

</vxml>